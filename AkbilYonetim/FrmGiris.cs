using System.Data.SqlClient;
using AkbilYonetimVeriKatmani;
using AkbilYonetimIsKatmani;
using System.Text;

namespace AkbilYonetim
{
    public partial class FrmGiris : Form
    {
        public string Email { get; set; } // Kayýt ol formunda kayýt olan kullanýcýnýn emaili buraya gelsin

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
            //Beni Hatýrlayý Properties.Settings ile yapana kadar burasý böyle kolaylýk saðlasýn.
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
                //1) Email ve þifre textboxlarý dolu mu?

                if (string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtSifre.Text))
                {
                    MessageBox.Show("Bilgileri eksiksiz giriniz !", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                //2) Girdiði email ve þifre veritabanýnda mevcut mu?
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
                    MessageBox.Show("Email ya da Þifre yanlýþ, tekrar deneyiniz !");
                }
                else
                {
                    GenelIslemler.GirisYapanKullaniciId = (int)sonuc["Id"];
                    GenelIslemler.GirisYapanKullaniciAdSoyad = $"{sonuc["Ad"]} {sonuc["Soyad"]}";
                    MessageBox.Show($"Hoþgeldiniz...{GenelIslemler.GirisYapanKullaniciAdSoyad}");

                    //BENÝ HATIRLA YAZILACAK
                    this.Hide();
                    FrmAnaSayfa frmAnaSayfa = new FrmAnaSayfa();
                    frmAnaSayfa.Show();
                }

            }
            catch (Exception hata)
            {
                //Dipnot : exceptionlar kullanýcýya gösterilmez, loglanýr. Biz þu an geliþtirme yaptýðýmýz için yazdýk
                MessageBox.Show("Beklenmedik bir sorun oluþtu !" + hata.Message);
            }
        }


        private void txtSifre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) //basýlan tuþ enter ise giriþ yapacak
            {
                GirisYap();
            }
        }
    }
}