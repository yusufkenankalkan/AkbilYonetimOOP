using AkbilYonetimIsKatmani;
using AkbilYonetimVeriKatmani;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AkbilYonetim
{
    public partial class FrmTalimatlar : Form
    {
        IVeriTabaniIslemleri veriTabaniIslemleri = new SQLVeriTabaniIslemleri(GenelIslemler.SinifSQLBaglantiCumlesi);
        public FrmTalimatlar()
        {
            InitializeComponent(); 
        }

        private void FrmTalimatlar_Load(object sender, EventArgs e)
        {
            //Comboxa akbilleri getir
            ComboBoxaKullanicininAkbilleriniGetir();
            cmbAkbiller.SelectedIndex = -1;
            grpBoxYukleme.Enabled = false;

            dataGridViewTalimatlar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            TalimatlariDataGrideGetir();

            chboxTumunuGoster.Checked = false;
            BekleyenTalimatSayisiniGetir();
            timerBekleyenTalimat.Interval = 1000;
            timerBekleyenTalimat.Enabled = true;
        }

        private void ComboBoxaKullanicininAkbilleriniGetir()
        {
            try
            {
                cmbAkbiller.DataSource = veriTabaniIslemleri.VeriGetir("Akbiller", kosullar:
                    $"AkbilSahibiId={GenelIslemler.GirisYapanKullaniciId}");
                cmbAkbiller.DisplayMember = "AkbilNo";
                cmbAkbiller.ValueMember = "AkbilNo"; //Genellikle benzersiz bilgi atanır. ÖRN:Primary key kolonu
            }
            catch (Exception hata)
            {

                MessageBox.Show("Beklenmedik bir hata oluştu !" + hata.Message);
            }
        }
        private void TalimatlariDataGrideGetir(bool tumunuGoster = false)
        {
            try
            {
                if (tumunuGoster)
                {
                    dataGridViewTalimatlar.DataSource = veriTabaniIslemleri.VeriGetir("KullanicininTalimatlari",
                        kosullar: $"KullaniciId = {GenelIslemler.GirisYapanKullaniciId}");
                }
                else
                {
                    dataGridViewTalimatlar.DataSource = veriTabaniIslemleri.VeriGetir("KullanicininTalimatlari",
                        kosullar: $"KullaniciId = {GenelIslemler.GirisYapanKullaniciId} and YuklendiMi = 0");
                }
                dataGridViewTalimatlar.Columns["Id"].Visible = false;
                dataGridViewTalimatlar.Columns["Akbil"].Width = 150;
                dataGridViewTalimatlar.Columns["YuklendiMi"].HeaderText = "Talimat Yüklendi Mi?";
                dataGridViewTalimatlar.Columns["YuklendiMi"].Width = 150;
                //İstenilen kolonlara gerekli özelleştirmeler yapılabilir

            }
            catch (Exception hata)
            {

                MessageBox.Show("Talimatlar getirilirken hata oluştu !" + hata.Message);
            }
        }
        private void BekleyenTalimatSayisiniGetir()
        {
            lblBekleyenTalimat.Text = veriTabaniIslemleri.VeriGetir("KullanicininTalimatlari", kosullar:
                $"KullaniciId={GenelIslemler.GirisYapanKullaniciId} and YuklendiMi = 0").Rows.Count.ToString();
        }

        private void cmbAkbiller_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAkbiller.SelectedIndex >= 0)
            {
                txtYuklenecekTutar.Clear();
                grpBoxYukleme.Enabled = true;
            }
            else
            {
                txtYuklenecekTutar.Clear();
                grpBoxYukleme.Enabled = false;
            }
            BekleyenTalimatSayisiniGetir();
            TalimatlariDataGrideGetir(chboxTumunuGoster.Checked);
        }

        private void anaSayfaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            FrmAnaSayfa frmAnaSayfa = new FrmAnaSayfa();
            frmAnaSayfa.Show();
        }

        private void btnTalimat_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbAkbiller.SelectedIndex < 0)
                {
                    MessageBox.Show("Akbil seçiniz !");
                    return;
                }
                if (string.IsNullOrEmpty(txtYuklenecekTutar.Text))
                {
                    MessageBox.Show("Yükleme tutarı girişi yapınız !");
                    return;
                }
                if (!decimal.TryParse(txtYuklenecekTutar.Text.Trim(), out decimal tutar))
                {
                    MessageBox.Show("Yükleme tutarı alanına uygun veri girişi yapınız !");
                    return;
                }
 
                Dictionary<string, object> kolonlar = new Dictionary<string, object>();
                kolonlar.Add("EklenmeTarihi", $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'");
                kolonlar.Add("AkbilID", $"'{cmbAkbiller.SelectedValue}'");
                kolonlar.Add("YuklenecekTutar", txtYuklenecekTutar.Text.Trim().Replace(",", "."));
                kolonlar.Add("YuklendiMi", "0");
                kolonlar.Add("YuklenmeTarihi", "null");

                string talimatinsert = veriTabaniIslemleri.VeriEklemeCumlesiOlustur("Talimatlar", kolonlar);
                int sonuc = veriTabaniIslemleri.KomutIsle(talimatinsert);
                if (sonuc > 0)
                {
                    MessageBox.Show("Talimat Kaydedildi...");
                    txtYuklenecekTutar.Clear();
                    cmbAkbiller.SelectedIndex = -1;
                    TalimatlariDataGrideGetir(chboxTumunuGoster.Checked);
                    BekleyenTalimatSayisiniGetir();
                }
            }
            catch (Exception hata)
            {

                MessageBox.Show("Beklenmedik bir hata oluştu !" + hata.Message);

            }
        }

        private void chboxTumunuGoster_CheckedChanged(object sender, EventArgs e)
        {
            TalimatlariDataGrideGetir(chboxTumunuGoster.Checked);
        }

        private void cikisYapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Çıkış yapıldı");
            GenelIslemler.GirisYapanKullaniciAdSoyad = string.Empty;
            GenelIslemler.GirisYapanKullaniciId = 0;

            foreach (Form item in Application.OpenForms)
            {
                if (item.Name != "FrmGiris")
                {
                    item.Hide();
                }
            }
            Application.OpenForms["FrmGiris"].Show();
        }

        private void timerBekleyenTalimat_Tick(object sender, EventArgs e)
        {
            if (lblBekleyenTalimat.Text != "0")
            {
                if (DateTime.Now.Second % 2 == 0)
                {
                    lblBekleyenTalimat.Font = new Font("Segoe UI", 30);
                    lblBekleyenTalimat.ForeColor = Color.Navy;
                }
                else
                {
                    lblBekleyenTalimat.Font = new Font("Segoe UI", 15);
                    lblBekleyenTalimat.ForeColor = Color.Navy;
                }
            }
        }

        private void talimatiIptalEtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int sayac = 0;
                foreach (DataGridViewRow item in dataGridViewTalimatlar.SelectedRows)
                {
                    //Yüklenmiş bir talimat iptal edilemez/silinemez.
                    if ((bool)item.Cells["YuklendiMi"].Value)
                    {
                        MessageBox.Show($"DİKKAT! {item.Cells["Akbil"].Value} {item.Cells["YuklenecekTutar"].Value} liralık yüklemesi yapılmıştır. YÜKLENEN TALİMAT İPTAL EDİLEMEZ/SİLİNEMEZ! \nİşlemlerinize devam etmek için tamama basınız.");
                        continue;
                    } // if bitti

                    sayac += veriTabaniIslemleri.VeriSil("Talimatlar", $"Id={item.Cells["Id"].Value}");

                } // foreach bitti

                MessageBox.Show($"Seçtiğiniz {sayac} adet talimat iptal edilmiştir.");
                TalimatlariDataGrideGetir();
                BekleyenTalimatSayisiniGetir();
            }
            catch (Exception hata)
            {
                MessageBox.Show("Beklenmedik bir sorun oluştu! " + hata.Message);
            }
        }

        private void talimatiYukleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int sayac = 0;
                foreach (DataGridViewRow item in dataGridViewTalimatlar.SelectedRows)
                {
                    //talimatlar tablosunu güncellemek
                    Hashtable talimatkolonlar = new Hashtable();
                    talimatkolonlar.Add("YuklendiMi", 1);
                    talimatkolonlar.Add("YuklenmeTarihi", $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'");
                    string talimatGuncelle = veriTabaniIslemleri.VeriGuncellemeCumlesiOlustur("Talimatlar", talimatkolonlar, $"Id={item.Cells["Id"].Value}");
                    if (veriTabaniIslemleri.KomutIsle(talimatGuncelle) > 0)
                    {
                        //akbilin mevcut bakiyesini öğren
                        decimal bakiye = Convert.ToDecimal(
                        veriTabaniIslemleri.VeriOku("Akbiller", new string[] { "Bakiye" },
                            $"AkbilNo='{item.Cells["Akbil"].Value.ToString()?.Substring(0, 16)}'")["Bakiye"]);

                        //var sonuc = veriTabaniIslemleri.VeriOku("Akbiller", new string[] { "Bakiye" },
                        //    $"AkbilNo='{item.Cells["Akbil"].Value.ToString()?.Substring(0, 15)}'");
                        //decimal bakiye = (decimal)sonuc["Bakiye"];

                        //akbil bakiyesini güncellemek
                        Hashtable akbilkolon = new Hashtable();

                        var sonBakiye = (bakiye + (decimal)item.Cells["YuklenecekTutar"].Value).ToString().Replace(",", ".");

                        akbilkolon.Add("Bakiye", sonBakiye);

                        string akbilGuncelle = veriTabaniIslemleri.VeriGuncellemeCumlesiOlustur("Akbiller", akbilkolon, $"AkbilNo='{item.Cells["Akbil"].Value.ToString()?.Substring(0, 16)}'");

                        sayac += veriTabaniIslemleri.KomutIsle(akbilGuncelle);
                    }
                } // foreach bitti.
                MessageBox.Show($"{sayac} adet talimat akbile yüklendi!");
                TalimatlariDataGrideGetir();
                BekleyenTalimatSayisiniGetir();
            }
            catch (Exception hata)
            {
                MessageBox.Show("Beklenmedik bir hata oluştu! " + hata.Message);
            }

        }
    }
}

