using AkbilYonetimIsKatmani;
using AkbilYonetimVeriKatmani;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AkbilYonetim
{
    public partial class FrmAkbiller : Form
    {
        IVeriTabaniIslemleri veriTabaniIslemleri = new SQLVeriTabaniIslemleri();

        public FrmAkbiller()
        {
            InitializeComponent();
        }


        private void FrmAkbiller_Load(object sender, EventArgs e)
        {
            cmbAkbilTipleri.Text = "Akbil tipi seçiniz...";
            cmbAkbilTipleri.SelectedIndex = -1;
            DataGridViewiDoldur();
        }


        private void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbAkbilTipleri.SelectedIndex < 0)
                {
                    MessageBox.Show("Lütfen eklemek istediğiniz akbil türünü seçiniz ! ");
                    return;
                }
                if (maskTxtAkbilNo.Text.Length < 16)
                {
                    MessageBox.Show("Akbil No 16 haneli olmalıdır !");
                    return;
                }

                Dictionary<string, object> yeniAkbil = new Dictionary<string, object>();
                yeniAkbil.Add("AkbilNo", $"'{maskTxtAkbilNo.Text}'");
                yeniAkbil.Add("Bakiye", 0);
                yeniAkbil.Add("AkbilTipi", $"'{cmbAkbilTipleri.SelectedItem}'");
                yeniAkbil.Add("EklenmeTarihi", $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'");
                yeniAkbil.Add("VizelendigiTarih", "null");
                yeniAkbil.Add("AkbilSahibiId", GenelIslemler.GirisYapanKullaniciId);

                string insertCumle = veriTabaniIslemleri.VeriEklemeCumlesiOlustur("Akbiller", yeniAkbil);
                int sonuc = veriTabaniIslemleri.KomutIsle(insertCumle);
                if (sonuc > 0)
                {
                    MessageBox.Show("Akbil Eklendi");
                    DataGridViewiDoldur();
                    maskTxtAkbilNo.Clear();
                    cmbAkbilTipleri.SelectedIndex = -1;
                    cmbAkbilTipleri.Text = "Akbil tipi seçiniz";
                }
                else
                {
                    MessageBox.Show("Akbil Eklenemedi");
                }

            }
            catch (Exception hata)
            {
                MessageBox.Show("Beklenmedik bir hata oluştu !" + hata.Message);
            }
        }

        private void DataGridViewiDoldur()
        {
            try
            {
                dataGridViewAkbiller.DataSource = veriTabaniIslemleri.VeriGetir("Akbiller", kosullar: $"AkbilSahibiId={GenelIslemler.GirisYapanKullaniciId}");

                // Bazı kolonlar gizlensin
                dataGridViewAkbiller.Columns["AkbilSahibiId"].Visible = false;
                dataGridViewAkbiller.Columns["VizelendigiTarih"].HeaderText = "Vizelendiği Tarih";
                dataGridViewAkbiller.Columns["VizelendigiTarih"].Width = 200;
            }
            catch (Exception hata)
            {
                MessageBox.Show("Akbilleri listeleyemedim !" + hata.Message);
            }
        }

        private void amaSayfaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            FrmAnaSayfa frmAnaSayfa = new FrmAnaSayfa();
            frmAnaSayfa.Show();
        }
    }
}
