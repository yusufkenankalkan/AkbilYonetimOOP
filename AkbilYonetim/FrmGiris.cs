using System.Data.SqlClient;
using AkbilYonetimVeriKatmani;
using AkbilYonetimIsKatmani;
using System.Text;

namespace AkbilYonetim
{
    public partial class FrmGiris : Form
    {
        public string Email { get; set; } // Kay�t ol formunda kay�t olan kullan�c�n�n emaili buraya gelsin

        IVeriTabaniIslemleri veriTabaniIslemleri = new SQLVeriTabaniIslemleri();
        public FrmGiris()
        {
            InitializeComponent();
        }
        private void FrmGiris_Load(object sender, EventArgs e)
        {

            txtEmail.TabIndex = 1;
            txtSifre.TabIndex = 2;
            checkBoxHatirla.TabIndex = 3;
            btnGirisYap.TabIndex = 4;
            btnKayitOl.TabIndex = 5;

            txtSifre.PasswordChar = '*';

            if (Email != null)
            {
                txtEmail.Text = Email;
            }
            //Beni Hat�rlay� Properties.Settings ile yapana kadar buras� b�yle kolayl�k sa�las�n.
            txtEmail.Text = "yusuf@gmail.com";
            txtSifre.Text = "1234";

        }
        private void btnKayitOl_Click(object sender, EventArgs e)
        {
            this.Hide();
            FrmKayitOl frm = new FrmKayitOl();
            frm.Show();
        }

        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            GirisYap();
        }

        private void GirisYap()
        {
            try
            {
                //1) Email ve �ifre textboxlar� dolu mu?

                if (string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtSifre.Text))
                {
                    MessageBox.Show("Bilgileri eksiksiz giriniz !", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                //2) Girdi�i email ve �ifre veritaban�nda mevcut mu?
                // select * from Kullanicilar where Email='' and Parola='' 

                string[] istedigimKolonlar = new string[] { "Id", "Ad", "Soyad" };
                string kosullar = string.Empty;
                StringBuilder sb = new StringBuilder();
                sb.Append($"Email='{txtEmail.Text.Trim()}'");
                sb.Append(" and ");
                sb.Append($"Parola='{GenelIslemler.MD5Encryption(txtSifre.Text.Trim())}'");
                kosullar = sb.ToString();

                var sonuc = veriTabaniIslemleri.VeriOku("Kullanicilar", istedigimKolonlar, kosullar);

                if (sonuc.Count == 0)
                {
                    MessageBox.Show("Email ya da �ifre yanl��, tekrar deneyiniz !");
                }
                else
                {
                    GenelIslemler.GirisYapanKullaniciId = (int)sonuc["Id"];
                    GenelIslemler.GirisYapanKullaniciAdSoyad = $"{sonuc["Ad"]} {sonuc["Soyad"]}";
                    MessageBox.Show($"Ho�geldiniz...{GenelIslemler.GirisYapanKullaniciAdSoyad}");

                    //BEN� HATIRLA YAZILACAK
                    this.Hide();
                    FrmAnaSayfa frmAnaSayfa = new FrmAnaSayfa();
                    frmAnaSayfa.Show();
                }

            }
            catch (Exception hata)
            {
                //Dipnot : exceptionlar kullan�c�ya g�sterilmez, loglan�r. Biz �u an geli�tirme yapt���m�z i�in yazd�k
                MessageBox.Show("Beklenmedik bir sorun olu�tu !" + hata.Message);
            }
        }


        private void txtSifre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) //bas�lan tu� enter ise giri� yapacak
            {
                GirisYap();
            }
        }
    }
}