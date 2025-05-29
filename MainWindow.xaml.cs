using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gra2D
{
    public partial class MainWindow : Window
    {
        
        // Stałe reprezentujące rodzaje terenu
        public const int LAS = 1;     // las
        public const int LAKA = 2;     // łąka
        public const int SKALA = 3;   // skały
        public const int WODA = 4;    // woda
        public const int MUSZELKI = 5; // muszelki
        public const int GLONY = 6;   // glony
        public const int REKIN = 7; //rekin
        public const int KILOF = 8; //kilof
        public const int GRUZ = 9; //gruz
        public const int GOLEM = 10; //golem
        public const int ILE_TERENOW = 11;   // ile terenów
        // Mapa przechowywana jako tablica dwuwymiarowa int
        private int[,] mapa;
        private int szerokoscMapy;
        private int wysokoscMapy;
        // Dwuwymiarowa tablica kontrolek Image reprezentujących segmenty mapy
        private Image[,] tablicaTerenu;
        // Rozmiar jednego segmentu mapy w pikselach
        private const int RozmiarSegmentu = 64;

        // Tablica obrazków terenu – indeks odpowiada rodzajowi terenu
        // Indeks 1: las, 2: łąka, 3: skały
        private BitmapImage[] obrazyTerenu = new BitmapImage[ILE_TERENOW];

        // Pozycja gracza na mapie
        private int pozycjaGraczaX = 0;
        private int pozycjaGraczaY = 0;
        // Obrazek reprezentujący gracza
        private Image obrazGracza;
        // Licznik zgromadzonego drewna
        private int iloscDrewna = 0;

        private int aktywnyPoziom = 1; //1 - las, 2 - woda
        private int iloscMuszelek = 6; //licznik muszelek
        private int jestRekin = 1; //zmienna do sprawdzenia czy jest rekin
        private int jestKilof = 0; //zmienna do sprawdzenia czy jest kilof
        private int jestGolem = 0; //zmienna do sprawdzenia czy jest golem
        public MainWindow()
        {
            InitializeComponent();
            WczytajObrazyTerenu();

            // Inicjalizacja obrazka gracza
            obrazGracza = new Image
            {
                Width = RozmiarSegmentu,
                Height = RozmiarSegmentu
            };
            BitmapImage bmpGracza = new BitmapImage(new Uri("gracz.png", UriKind.Relative));
            obrazGracza.Source = bmpGracza;
        }
        private void WczytajObrazyTerenu()
        {
            // Zakładamy, że tablica jest indeksowana od 0, ale używamy indeksów 1-3
            obrazyTerenu[LAS] = new BitmapImage(new Uri("las.png", UriKind.Relative));
            obrazyTerenu[LAKA] = new BitmapImage(new Uri("laka.png", UriKind.Relative));
            obrazyTerenu[SKALA] = new BitmapImage(new Uri("skala.png", UriKind.Relative));
            obrazyTerenu[WODA] = new BitmapImage(new Uri("woda.png", UriKind.Relative));
            obrazyTerenu[MUSZELKI] = new BitmapImage(new Uri("muszelka.png", UriKind.Relative));
            obrazyTerenu[GLONY] = new BitmapImage(new Uri("glony.png", UriKind.Relative));
            obrazyTerenu[REKIN] = new BitmapImage(new Uri("rekin.png", UriKind.Relative));
            obrazyTerenu[KILOF] = new BitmapImage(new Uri("kilof.png", UriKind.Relative));
            obrazyTerenu[GRUZ] = new BitmapImage(new Uri("gruz.png", UriKind.Relative));
            obrazyTerenu[GOLEM] = new BitmapImage(new Uri("golem.png", UriKind.Relative));

        }

        // Wczytuje mapę z pliku tekstowego i dynamicznie tworzy tablicę kontrolek Image
        private void WczytajMape(string sciezkaPliku)
        {
            try
            {
                var linie = File.ReadAllLines(sciezkaPliku);//zwraca tablicę stringów, np. linie[0] to pierwsza linia pliku
                wysokoscMapy = linie.Length;
                szerokoscMapy = linie[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;//zwraca liczbę elementów w tablicy
                mapa = new int[wysokoscMapy, szerokoscMapy];

                for (int y = 0; y < wysokoscMapy; y++)
                {
                    var czesci = linie[y].Split(' ', StringSplitOptions.RemoveEmptyEntries);//zwraca tablicę stringów np. czesci[0] to pierwszy element linii
                    for (int x = 0; x < szerokoscMapy; x++)
                    {
                        mapa[y, x] = int.Parse(czesci[x]);//wczytanie mapy z pliku
                    }
                }

                // Przygotowanie kontenera SiatkaMapy – czyszczenie elementów i definicji wierszy/kolumn
                SiatkaMapy.Children.Clear();
                SiatkaMapy.RowDefinitions.Clear();
                SiatkaMapy.ColumnDefinitions.Clear();

                for (int y = 0; y < wysokoscMapy; y++)
                {
                    SiatkaMapy.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(RozmiarSegmentu) });
                }
                for (int x = 0; x < szerokoscMapy; x++)
                {
                    SiatkaMapy.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(RozmiarSegmentu) });
                }

                // Tworzenie tablicy kontrolk Image i dodawanie ich do siatki
                tablicaTerenu = new Image[wysokoscMapy, szerokoscMapy];
                for (int y = 0; y < wysokoscMapy; y++)
                {
                    for (int x = 0; x < szerokoscMapy; x++)
                    {
                        Image obraz = new Image
                        {
                            Width = RozmiarSegmentu,
                            Height = RozmiarSegmentu
                        };

                        int rodzaj = mapa[y, x];
                        if (rodzaj >= 1 && rodzaj < ILE_TERENOW)
                        {
                            obraz.Source = obrazyTerenu[rodzaj];//wczytanie obrazka terenu
                        }
                        else
                        {
                            obraz.Source = null;
                        }

                        Grid.SetRow(obraz, y);
                        Grid.SetColumn(obraz, x);
                        SiatkaMapy.Children.Add(obraz);//dodanie obrazka do siatki na ekranie
                        tablicaTerenu[y, x] = obraz;
                    }
                }
                bool zawieraWode = false; //Sprawdzenie czy mapa zawiera wodę(poziom morski)
                foreach (int pole in mapa)
                {
                    if (pole == WODA || pole == MUSZELKI || pole == GLONY)
                    {
                        zawieraWode = true;
                        break;
                    }
                }
                aktywnyPoziom = zawieraWode ? 2 : 1; //1 - las, 2 - woda
                // Dodanie obrazka gracza – ustawiamy go na wierzchu
                SiatkaMapy.Children.Add(obrazGracza);
                Panel.SetZIndex(obrazGracza, 1);//ustawienie obrazka gracza na wierzchu
                pozycjaGraczaX = 0;
                pozycjaGraczaY = 0;
                AktualizujPozycjeGracza();

                iloscDrewna = 0;
                EtykietaDrewna.Content = "Drewno: " + iloscDrewna;
            }//koniec try
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania mapy: " + ex.Message);
            }
        }

        // Aktualizuje pozycję obrazka gracza w siatce
        private void AktualizujPozycjeGracza(bool zmienMape = false, string sciezka = "../../../level-1.txt")
        {
            Grid.SetRow(obrazGracza, pozycjaGraczaY);
            Grid.SetColumn(obrazGracza, pozycjaGraczaX);
            if (zmienMape == true)
            {
                try
                {
                    WczytajMape(sciezka);
                    pozycjaGraczaX = 0;
                    pozycjaGraczaY = 0;
                    if (SiatkaMapy.Children.Contains(obrazGracza))
                    {
                        SiatkaMapy.Children.Remove(obrazGracza);
                    }
                    SiatkaMapy.Children.Add(obrazGracza);
                    Panel.SetZIndex(obrazGracza, 1);
                    jestGolem = 0;
                    AktualizujPozycjeGracza();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd wczytywania mapy: " + ex.Message);
                }

            }

        }

        // Obsługa naciśnięć klawiszy – ruch gracza oraz wycinanie lasu
        private void OknoGlowne_KeyDown(object sender, KeyEventArgs e)
        {
            int nowyX = pozycjaGraczaX;
            int nowyY = pozycjaGraczaY;
            //zmiana pozycji gracza
            if (e.Key == Key.Up) nowyY--;
            else if (e.Key == Key.Down) nowyY++;
            else if (e.Key == Key.Left) nowyX--;
            else if (e.Key == Key.Right) nowyX++;
            //Gracz nie może wyjść poza mapę
            if (nowyX >= 0 && nowyX < szerokoscMapy && nowyY >= 0 && nowyY < wysokoscMapy)
            {
                int pole = mapa[nowyY, nowyX];//dodajemy pole na którym jest gracz
                if (aktywnyPoziom == 1) //Poziom 1 - LAS
                {
                    // Obsługa wycinania lasu – naciskamy klawisz C
                    if (e.Key == Key.C && mapa[pozycjaGraczaY, pozycjaGraczaX] == LAS) //jesli gracz jest na polu las to działa ta funkcja 
                    {
                       Scinanie();

                    }
                    // Gracz nie może wejść na pole ze skałami
                    if (pole != SKALA)
                    {
                       ZakazWejsciaNaSkaly(nowyY, nowyX);

                    }
                    if (pole == KILOF)
                    {
                       ZdobycieKilofa(nowyY, nowyX);
                    }
                    if (jestKilof == 1 && pole == SKALA)
                    {
                       ZamianaPolaNaGruz(nowyY, nowyX);
                    }
                    int licznikGruzu = 0;

                    for (int y = 0; y < wysokoscMapy; y++) //Dodawanie pola gruz w miejsce gracza
                    {
                        for (int x = 0; x < szerokoscMapy; x++)
                        {
                            if (mapa[y, x] == GRUZ)
                            {
                                licznikGruzu++;
                            }
                        }
                    }

                    if (licznikGruzu >= 3 && jestGolem == 0)
                    {
                        PojawiaSieGolem();
                    }
                    if (pole == GOLEM)
                    {
                        wejscieNaGolema(nowyX, nowyY);
                    }
                    
                   
                }
                if (aktywnyPoziom == 2) //Poziom 2 - WODA
                {

                    //Zbieranie muszelek
                    if (e.Key == Key.C && mapa[pozycjaGraczaY, pozycjaGraczaX] == MUSZELKI)
                    {
                       ZbieranieMuszelek();
                    }
                    //gracz nie może wejść na pole z glonami
                    if (pole != GLONY)
                    {                    
                        ZakazWejsciaNaGlony(nowyY, nowyX);
                    }
                    if (pole == REKIN)  //funkcja jeśli gracz wpadnie na rekina
                    {
                       WejscieNaRekina(nowyY, nowyX);
                    }
                }
            }

        }

        private void wejscieNaGolema(int nowyX, int nowyY)
           {
            MessageBox.Show("Uwaga! Golem! Zgubiłeś całe drewno!");
            EtykietaDrewna.Content = "Drewno: " + iloscDrewna;
            mapa[pozycjaGraczaY, pozycjaGraczaX] = LAKA; //zmiana na pole LAKA
            tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[LAKA];
            pozycjaGraczaX = nowyX;
            pozycjaGraczaY = nowyY;
            iloscDrewna = 0;
            AktualizujPozycjeGracza(true, "../../../level-1.txt");
            EtykietaDrewna.Content = "Drewno: " + iloscDrewna;
            jestKilof = 0;
        }
        private void PojawiaSieGolem()
        {
            MessageBox.Show("Pojawił sie golem!");
            mapa[0, 0] = GOLEM; //zmiana na pole GOLEM
            tablicaTerenu[0, 0].Source = obrazyTerenu[GOLEM];
            jestGolem = 1;
        }
        private void ZamianaPolaNaGruz(int nowyY, int nowyX)
        {
            mapa[nowyY, nowyX] = GRUZ; //zmiana na pole GRUZ
            tablicaTerenu[nowyY, nowyX].Source = obrazyTerenu[GRUZ];
            pozycjaGraczaX = nowyX;
            pozycjaGraczaY = nowyY;
            AktualizujPozycjeGracza();
        }
        private void ZdobycieKilofa(int nowyY, int nowyX)
        {
            MessageBox.Show("Znalazłeś kilof! Możesz teraz kopać w skałach!");
            mapa[pozycjaGraczaY, pozycjaGraczaX] = LAKA; //zmiana na pole LAKA
            tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[LAKA];
            pozycjaGraczaX = nowyX;
            pozycjaGraczaY = nowyY;
            jestKilof = 1;
            AktualizujPozycjeGracza();
        }
        private void Wygrana1Poziom()
        {
            MessageBox.Show("Gratulacje! Udało Ci się zebrać 7 drewna! Wygrałeś pierwszy poziom!");
            AktualizujPozycjeGracza(true, "../../../level-2.txt");
            body.Background = Brushes.Navy;
            EtykietaMuszelek.Content = "Muszelki: " + iloscMuszelek;
            jestKilof = 0;
            jestGolem = 0;
            aktywnyPoziom = 2; //zmiana poziomu na 2
        }
        private void Scinanie()
        {
            mapa[pozycjaGraczaY, pozycjaGraczaX] = LAKA;
            tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[LAKA];
            iloscDrewna++;
            EtykietaDrewna.Content = "Drewno: " + iloscDrewna;
        }
        private void ZakazWejsciaNaSkaly(int nowyY, int nowyX)
        {
            if (iloscDrewna == 10 && jestGolem == 1)
            {
                Wygrana1Poziom();
            }
            pozycjaGraczaX = nowyX;
            pozycjaGraczaY = nowyY;
            AktualizujPozycjeGracza();
        }
        private void ZbieranieMuszelek()
        {
            mapa[pozycjaGraczaY, pozycjaGraczaX] = WODA; //zmiana na pole WODA
            tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[WODA];
            iloscMuszelek--;
            EtykietaMuszelek.Content = "Muszelki: " + iloscMuszelek;
        }
        private void Wygrana2Poziom()
        {
            MessageBox.Show("Gratulacje! Wygrałeś!");
            Close();
        }
        private void ZakazWejsciaNaGlony(int nowyY, int nowyX)
        {
            if (iloscMuszelek == 0 && jestRekin == 1)
            {
                Wygrana2Poziom();
            }

            pozycjaGraczaX = nowyX;
            pozycjaGraczaY = nowyY;
            AktualizujPozycjeGracza();
        }
        private void WejscieNaRekina(int nowyY, int nowyX)
        {
            MessageBox.Show("Uwaga! Rekin! Zgubiłeś wszystkie muszelki!");
            EtykietaMuszelek.Content = "Muszelki: " + iloscMuszelek; // ile muszelek gdy wpadnie na rekina
            mapa[pozycjaGraczaY, pozycjaGraczaX] = WODA; //zmiana na pole WODA
            tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[WODA];
            pozycjaGraczaX = nowyX;
            pozycjaGraczaY = nowyY;
            jestRekin = 1; //rekin zostaje
            iloscMuszelek = 6;
            AktualizujPozycjeGracza(true, "../../../level-2.txt"); // mapa sie resetuje
            EtykietaMuszelek.Content = "Muszelki: " + iloscMuszelek;// resetuje licznik muszelek
        }

        // Obsługa przycisku "Wczytaj mapę"
        private void WczytajMape_Click(object sender, RoutedEventArgs e)
        {
            //AktualizujPozycjeGracza(true, "../../../level-1.txt");
        }

        private void jak_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("postać porusza sie strzałkami i zbiera klikajać przycisk C");
            return;
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            menu.Visibility = Visibility.Collapsed;
            gra.Visibility = Visibility.Visible;
            AktualizujPozycjeGracza(true, "../../../level-1.txt");
        }
    }
}


