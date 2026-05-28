using BlaisePascal.CdManagement.Domain;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlaisePascal.CDManagement.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<Song> _brani = new();
        private readonly List<CD> _cdList = new();

        public MainWindow()
        {
            InitializeComponent();
            RefreshUI();
        }

        // ─── CREAZIONE BRANO ──────────────────────────────────────────────────────
        private void BtnCreaBrano_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string titolo = TxtBranoTitolo.Text.Trim();
                string artista = TxtBranoArtista.Text.Trim();
                string durataRaw = TxtBranoDurata.Text.Trim();

                if (string.IsNullOrEmpty(titolo))
                    throw new ArgumentException("Il campo 'Titolo' è obbligatorio.");
                if (string.IsNullOrEmpty(artista))
                    throw new ArgumentException("Il campo 'Artista' è obbligatorio.");
                if (!int.TryParse(durataRaw, out int durata) || durata <= 0)
                    throw new ArgumentException("La durata deve essere un numero intero positivo (es. 210).");

                var brano = new Song(titolo, artista, durata);
                _brani.Add(brano);

                TxtBranoTitolo.Clear();
                TxtBranoArtista.Clear();
                TxtBranoDurata.Clear();
                TxtBranoTitolo.Focus();

                RefreshUI();
                SetStatus($"✅ Brano \"{brano.GetTitle()}\" ({brano.GetDuration()}) creato con successo.", success: true);
            }
            catch (ArgumentException ex)
            {
                ShowError("Errore creazione brano", ex.Message);
            }
            catch (Exception ex)
            {
                ShowError("Errore imprevisto", ex.Message);
            }
        }

        // ─── CREAZIONE CD ─────────────────────────────────────────────────────────
        private void BtnCreaCD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string titolo = TxtCdTitolo.Text.Trim();
                string artista = TxtCdArtista.Text.Trim();

                if (string.IsNullOrEmpty(titolo))
                    throw new ArgumentException("Il campo 'Titolo CD' è obbligatorio.");
                if (string.IsNullOrEmpty(artista))
                    throw new ArgumentException("Il campo 'Artista CD' è obbligatorio.");

                var cd = new CD(titolo, artista, new List<Song>());
                _cdList.Add(cd);

                TxtCdTitolo.Clear();
                TxtCdArtista.Clear();

                RefreshUI();
                // Seleziona automaticamente il CD appena creato
                CmbCD.SelectedItem = cd;
                SetStatus($"✅ CD \"{cd.GetTitle()}\" creato con successo.", success: true);
            }
            catch (ArgumentException ex)
            {
                ShowError("Errore creazione CD", ex.Message);
            }
            catch (Exception ex)
            {
                ShowError("Errore imprevisto", ex.Message);
            }
        }

        // ─── AGGIUNGI BRANO AL CD ─────────────────────────────────────────────────
        private void BtnAggiungiBrano_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cd = CmbCD.SelectedItem as CD;
                var brano = CmbBrano.SelectedItem as Song;

                if (cd == null) throw new InvalidOperationException("Seleziona un CD dal menu a tendina.");
                if (brano == null) throw new InvalidOperationException("Seleziona un brano dal menu a tendina.");

                cd.AddSong(brano);
                RefreshBraniCD(cd);
                SetStatus($"✅ \"{brano.GetTitle()}\" aggiunto a \"{cd.GetTitle()}\" — {cd.Songs.Count} brani, {cd.GetDuration()}.", success: true);
            }
            catch (InvalidOperationException ex)
            {
                ShowError("Operazione non valida", ex.Message);
            }
            catch (Exception ex)
            {
                ShowError("Errore imprevisto", ex.Message);
            }
        }

        // ─── RIMUOVI BRANO DAL CD ─────────────────────────────────────────────────
        private void BtnRimuoviBrano_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cd = CmbCD.SelectedItem as CD;
                var brano = ListBraniCD.SelectedItem as Song;

                if (cd == null)
                    throw new InvalidOperationException("Seleziona un CD dal menu a tendina.");
                if (brano == null)
                    throw new InvalidOperationException("Seleziona un brano nella lista del CD (colonna destra).");

                bool rimosso = cd.Songs.Remove(brano);
                if (rimosso)
                {
                    RefreshBraniCD(cd);
                    SetStatus($"🗑️ \"{brano.GetTitle()}\" rimosso da \"{cd.GetTitle()}\".", success: true);
                }
                else
                {
                    ShowError("Brano non trovato", "Il brano selezionato non è presente nel CD.");
                }
            }
            catch (InvalidOperationException ex)
            {
                ShowError("Operazione non valida", ex.Message);
            }
            catch (Exception ex)
            {
                ShowError("Errore imprevisto", ex.Message);
            }
        }

        // ─── RICERCA ARTISTA ──────────────────────────────────────────────────────
        private void BtnCercaArtista_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cd = CmbCD.SelectedItem as CD;
                if (cd == null)
                {
                    TxtRisultatoRicerca.Text = "⚠️ Seleziona prima un CD.";
                    return;
                }

                string artista = TxtRicercaArtista.Text.Trim();
                if (string.IsNullOrEmpty(artista))
                {
                    TxtRisultatoRicerca.Text = "⚠️ Inserisci un nome artista.";
                    return;
                }

                var risultati = cd.GetByAuthor(artista);
                if (risultati.Count == 0)
                {
                    TxtRisultatoRicerca.Text = $"Nessun brano di \"{artista}\" trovato nel CD.";
                }
                else
                {
                    var lines = risultati.Select(b => $"• {b.GetTitle()} ({b.GetDuration()})");
                    TxtRisultatoRicerca.Text =
                        $"Trovati {risultati.Count} brano/i di \"{artista}\":\n" +
                        string.Join("\n", lines);
                }
            }
            catch (Exception ex)
            {
                TxtRisultatoRicerca.Text = $"❌ Errore: {ex.Message}";
            }
        }

        // ─── EVENTI UI ────────────────────────────────────────────────────────────
        private void CmbCD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cd = CmbCD.SelectedItem as CD;
            RefreshBraniCD(cd);
            TxtRisultatoRicerca.Text = string.Empty;
        }

        private void ListBrani_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBrani.SelectedItem is Song brano)
                CmbBrano.SelectedItem = brano;
        }

        // ─── REFRESH UI ──────────────────────────────────────────────────────────
        private void RefreshUI()
        {
            // Salva selezioni correnti
            var selectedCd = CmbCD.SelectedItem as CD;
            var selectedBrano = CmbBrano.SelectedItem as Song;

            // Aggiorna lista brani
            ListBrani.ItemsSource = null;
            ListBrani.ItemsSource = _brani;
            TxtCountBrani.Text = $"({_brani.Count} totali)";

            // Aggiorna combo brano — DisplayMemberPath gestito dal DataTemplate nello XAML
            CmbBrano.ItemsSource = null;
            CmbBrano.ItemsSource = _brani;

            // Aggiorna combo CD
            CmbCD.ItemsSource = null;
            CmbCD.ItemsSource = _cdList;

            // Ripristina selezioni
            if (selectedCd != null && _cdList.Contains(selectedCd))
                CmbCD.SelectedItem = selectedCd;
            else if (_cdList.Count > 0)
                CmbCD.SelectedIndex = 0;

            if (selectedBrano != null && _brani.Contains(selectedBrano))
                CmbBrano.SelectedItem = selectedBrano;

            RefreshBraniCD(CmbCD.SelectedItem as CD);
        }

        private void RefreshBraniCD(CD? cd)
        {
            ListBraniCD.ItemsSource = null;

            if (cd == null)
            {
                TxtInfoCD.Text = "Nessun CD selezionato";
                return;
            }

            ListBraniCD.ItemsSource = cd.Songs;

            TxtInfoCD.Text = $"| {cd.GetTitle()}  · {cd.Songs.Count} brani · {cd.GetDuration()} / 80:00 ";
        }

        // ─── HELPERS ─────────────────────────────────────────────────────────────
        private void SetStatus(string msg, bool success = false)
        {
            TxtStatus.Text = msg;
            TxtStatus.Foreground = success
                ? Brushes.LightGreen
                : new SolidColorBrush(Color.FromRgb(144, 202, 249)); // #90CAF9
        }

        private void ShowError(string titolo, string msg)
        {
            SetStatus($"❌ {msg}");
            MessageBox.Show(msg, titolo, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}