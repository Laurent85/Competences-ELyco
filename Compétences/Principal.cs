﻿using Compétences.Properties;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Application = Microsoft.Office.Interop.Word.Application;
using CheckBox = System.Windows.Forms.CheckBox;
using ListBox = System.Windows.Forms.ListBox;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace Compétences
{
    public partial class Principal : Form
    {
        public string CheminElyco = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public Message MessageTraitement = new Message();

        public Principal()
        {
            InitializeComponent();
        }

        private void OuvertureLogiciel(object sender, EventArgs e)
        {
            Directory.CreateDirectory(CheminElyco + @"\ELyco");
            Directory.CreateDirectory(CheminElyco + @"\ELyco\Config");
            Directory.CreateDirectory(CheminElyco + @"\ELyco\Backup");
            File.WriteAllText(CheminElyco + @"\ELyco\Config\ELyco_classes.txt", string.Empty);
            File.WriteAllText(CheminElyco + @"\ELyco\Config\ELyco_classes_annee.txt", string.Empty);
            File.WriteAllText(CheminElyco + @"\ELyco\Config\ELyco_classes_dnb.txt", string.Empty);
            BtnLancerTraitement.Enabled = false;
            BtnSuppressionFichierCsvATraiter.Enabled = false;
            BtnSuppressionFichierCsv.Enabled = false;
            BtnSuppressionFichierXlsx.Enabled = false;
            BtnGénérerfichiersExcelDnb.Enabled = false;
            BtnGénérerPublipostageDnb.Enabled = false;
            RafraichirListbox();
            ChkDocx.Checked = true;
            ChkPdf.Checked = true;
            ChkXlsx.Checked = true;
            ChkCsv.Checked = true;
            CacherFichiersXlsxDocx();

            foreach (var listBoxItem in ListBoxCsvATraiter.Items)
            {
                if (!File.Exists(LblCheminDossierCsv.Text + @"\" + Path.GetFileName(listBoxItem.ToString())))
                    File.Copy(listBoxItem.ToString(),
                        LblCheminDossierCsv.Text + @"\" + Path.GetFileName(listBoxItem.ToString()));

                File.AppendAllText(CheminElyco + @"\ELyco\Config\ELyco_classes.txt",
                    Path.GetFileName(listBoxItem.ToString()).Substring(25, 2) + Environment.NewLine);
            }

            try
            {
                ComboAnnéeScolaire.Text =
                    File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(2).Take(3).First();
                ComboNiveau6.Text =
                    File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(3).Take(4).First() + @" classes";
                ComboNiveau5.Text =
                    File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(4).Take(5).First() + @" classes";
                ComboNiveau4.Text =
                    File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(5).Take(6).First() + @" classes";
                ComboNiveau3.Text =
                    File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(6).Take(7).First() + @" classes";
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void BtnCheminCsv(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var chemin = dlg.SelectedPath + @"\ELyco_CSV\" + ComboAnnéeScolaire.SelectedItem;
                LblCheminDossierCsv.Text = chemin;
                Directory.CreateDirectory(CheminElyco + @"\ELyco\Config");

                if (!File.Exists(CheminElyco + @"\ELyco\Config\ELyco_in.txt"))
                    using (File.Create(CheminElyco + @"\ELyco\Config\ELyco_in.txt"))
                    {
                    }
                if (!File.Exists(CheminElyco + @"\ELyco\Config\ELyco_classes.txt"))
                    using (File.Create(CheminElyco + @"\ELyco\Config\ELyco_classes.txt"))
                    {
                    }
                if (!File.Exists(CheminElyco + @"\ELyco\Config\ELyco_classes_annee.txt"))
                    using (File.Create(CheminElyco + @"\ELyco\Config\ELyco_classes_annee.txt"))
                    {
                    }
                if (!File.Exists(CheminElyco + @"\ELyco\Config\ELyco_classes_dnb.txt"))
                    using (File.Create(CheminElyco + @"\ELyco\Config\ELyco_classes_dnb.txt"))
                    {
                    }
                using (var sw = new StreamWriter(CheminElyco + @"\ELyco\Config\ELyco_in.txt"))
                {
                    sw.WriteLine(LblCheminDossierCsv.Text);
                    sw.WriteLine(dlg.SelectedPath + @"\ELyco_CSV" + "\n");
                }
            }
        }

        private void BtnCheminXlsx(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var cheminXlsx = dlg.SelectedPath + @"\ELyco_Competences\" + ComboAnnéeScolaire.SelectedItem;
                LblCheminDossierXlsx.Text = cheminXlsx;
                Directory.CreateDirectory(CheminElyco + @"\ELyco\Config");

                if (!File.Exists(CheminElyco + @"\ELyco\Config\ELyco_out.txt"))
                    using (File.Create(CheminElyco + @"\ELyco\Config\ELyco_out.txt"))
                    {
                    }
                using (var sw = new StreamWriter(CheminElyco + @"\ELyco\Config\ELyco_out.txt"))
                {
                    sw.WriteLine(LblCheminDossierXlsx.Text);
                    sw.WriteLine(dlg.SelectedPath + @"\ELyco_Competences");
                }
            }
        }

        private void BtnCréerArborescence(object sender, EventArgs e)
        {
            CréationArborescence("6");
            CréationArborescence("5");
            CréationArborescence("4");
            CréationArborescence("3");

            Directory.CreateDirectory(LblCheminDossierXlsx.Text + @"\" + "1ère période");
            Directory.CreateDirectory(LblCheminDossierXlsx.Text + @"\" + "2ème période");
            Directory.CreateDirectory(LblCheminDossierXlsx.Text + @"\" + "3ème période");
            Directory.CreateDirectory(LblCheminDossierXlsx.Text + @"\" + "Année");
            Directory.CreateDirectory(LblCheminDossierXlsx.Text + @"\" + "DNB");

            var file = new StreamReader(CheminElyco + @"\ELyco\Config\ELyco_in.txt");
            var nbLignes = 0;
            while (file.ReadLine() != null)
                nbLignes++;
            file.Close();

            if (nbLignes < 7)
            {
                ChangerLigneFichierTxt(ComboAnnéeScolaire.SelectedItem + "\n",
                    CheminElyco + @"\ELyco\Config\ELyco_in.txt", 3);
                ChangerLigneFichierTxt(ComboNiveau6.SelectedItem + "\n", CheminElyco + @"\ELyco\Config\ELyco_in.txt",
                    4);
                ChangerLigneFichierTxt(ComboNiveau5.SelectedItem + "\n", CheminElyco + @"\ELyco\Config\ELyco_in.txt",
                    5);
                ChangerLigneFichierTxt(ComboNiveau4.SelectedItem + "\n", CheminElyco + @"\ELyco\Config\ELyco_in.txt",
                    6);
                ChangerLigneFichierTxt(ComboNiveau3.SelectedItem + "\n", CheminElyco + @"\ELyco\Config\ELyco_in.txt",
                    7);
            }
            else
            {
                ChangerLigneFichierTxt(ComboAnnéeScolaire.SelectedItem.ToString(),
                    CheminElyco + @"\ELyco\Config\ELyco_in.txt", 3);
                ChangerLigneFichierTxt(ComboNiveau6.SelectedItem.ToString(),
                    CheminElyco + @"\ELyco\Config\ELyco_in.txt", 4);
                ChangerLigneFichierTxt(ComboNiveau5.SelectedItem.ToString(),
                    CheminElyco + @"\ELyco\Config\ELyco_in.txt", 5);
                ChangerLigneFichierTxt(ComboNiveau4.SelectedItem.ToString(),
                    CheminElyco + @"\ELyco\Config\ELyco_in.txt", 6);
                ChangerLigneFichierTxt(ComboNiveau3.SelectedItem.ToString(),
                    CheminElyco + @"\ELyco\Config\ELyco_in.txt", 7);
            }

            OuvertureLogiciel(sender, e);
        }

        private void BtnTraitementCsv(object sender, EventArgs e)
        {
            VérifierDoublonClasseCsv(DétectionPériode());

            var traitementMacro = new BackgroundWorker();
            traitementMacro.DoWork += DébutMacroCompétences;
            traitementMacro.RunWorkerCompleted += FinMacroCompétences;
            traitementMacro.RunWorkerAsync();
            traitementMacro.WorkerSupportsCancellation = true;

            if (LblFichiersCsvATraiter.Text == @"0 classes à traiter")

                MessageTraitement.Controls.Find("LblMessageTraitement", true).First().Text =
                    ListBoxCsvPrésents.SelectedItems.Count +
                    @" classes à traiter...Veuillez patienter...";
            else
                MessageTraitement.Controls.Find("LblMessageTraitement", true).First().Text =
                    LblFichiersCsvATraiter.Text + @"...Veuillez patienter...";

            MessageTraitement.Controls.Find("BtnFermerMessageTraitement", true).First().Visible = false;
            MessageTraitement.ShowDialog();
        }

        private void DébutMacroCompétences(object sender, DoWorkEventArgs e)
        {
            if (RadioBtnPériode1.Checked)
            {
                ExécuterMacro("Deplacer_P1.Deplacer_P1");
                ExécuterMacro("Compétences_par_lot_P1.Compétences_par_lot_P1");
                ConvertirXlsxEnPdf("1ère période");
                CacherFichiersXlsxDocx();
            }
            if (RadioBtnPériode2.Checked)
            {
                ExécuterMacro("Deplacer_P2.Deplacer_P2");
                ExécuterMacro("Compétences_par_lot_P2.Compétences_par_lot_P2");
                ConvertirXlsxEnPdf("2ème période");
                CacherFichiersXlsxDocx();
            }
            if (RadioBtnPériode3.Checked)
            {
                ExécuterMacro("Deplacer_P3.Deplacer_P3");
                ExécuterMacro("Compétences_par_lot_P3.Compétences_par_lot_P3");
                ConvertirXlsxEnPdf("3ème période");
                CacherFichiersXlsxDocx();
            }
            if (RadioBtnAnnée.Checked)
            {
                ExécuterMacro("Fusionner.Fusionner");
                ExécuterMacro("Compétences_par_lot_Année.Compétences_par_lot_Année");
                ConvertirXlsxEnPdf("Année");
                CacherFichiersXlsxDocx();
            }
        }

        private void FinMacroCompétences(object sender, RunWorkerCompletedEventArgs e)
        {
            RafraichirListbox();

            var files = Directory.GetFiles(LblCheminDossierCsv.Text, "*.csv");
            foreach (var file in files)
                File.Delete(file);

            File.WriteAllText(CheminElyco + @"\ELyco\Config\ELyco_classes.txt", string.Empty);

            MessageTraitement.Controls.Find("LblMessageTraitement", true).First().Text =
                @"Traitement des fichiers terminé !";
            MessageTraitement.Controls.Find("BtnFermerMessageTraitement", true).First().Visible = true;
        }

        private void BtnTraitementDnb(object sender, EventArgs e)
        {
            var traitementMacro = new BackgroundWorker();
            traitementMacro.DoWork += DébutMacroDnb;
            traitementMacro.RunWorkerCompleted += FinMacroDnb;
            traitementMacro.RunWorkerAsync();
            traitementMacro.WorkerSupportsCancellation = true;

            MessageTraitement.Controls.Find("LblMessageTraitement", true).First().Text =
                ListBoxXlsxPrésents.SelectedItems.Count +
                @" classes à traiter...Veuillez patienter...";

            MessageTraitement.Controls.Find("BtnFermerMessageTraitement", true).First().Visible = false;
            MessageTraitement.ShowDialog();
        }

        private void DébutMacroDnb(object sender, DoWorkEventArgs e)
        {
            ExécuterMacro("Publipostage.Publipostage");

            foreach (var fichierSélectionné in ListBoxXlsxPrésents.SelectedItems)
                if (fichierSélectionné.ToString().Contains("xlsx"))
                {
                    var appWord = new Application();
                    var strPath = LblCheminDossierXlsx.Text + @"DNB\" + fichierSélectionné;
                    var nomFichier = Path.GetFileNameWithoutExtension(strPath);
                    var wordDocument =
                        appWord.Documents.Open(LblCheminDossierXlsx.Text + @"DNB\" + nomFichier + @".docx");
                    wordDocument.ExportAsFixedFormat(LblCheminDossierXlsx.Text + @"DNB\" + nomFichier + @".pdf",
                        WdExportFormat.wdExportFormatPDF);
                    appWord.Documents.Close();
                    appWord.Quit();
                    GC.Collect();
                }
        }

        private void FinMacroDnb(object sender, RunWorkerCompletedEventArgs e)
        {
            RafraichirListbox();
            MessageTraitement.Controls.Find("LblMessageTraitement", true).First().Text =
                @"Traitement des fichiers terminé !";
            MessageTraitement.Controls.Find("BtnFermerMessageTraitement", true).First().Visible = true;
            CacherFichiersXlsxDocx();
        }

        private void BtnGénérerfichiersExcelDnb_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(this, @"S'agit-il du DNB N°1 ?", @"Confirmation", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                var traitementMacro = new BackgroundWorker();
                traitementMacro.DoWork += DébutGénérerfichiersExcelDnb1;
                traitementMacro.RunWorkerCompleted += FinGénérerfichiersExcelDnb;
                traitementMacro.RunWorkerAsync();
                traitementMacro.WorkerSupportsCancellation = true;

                MessageTraitement.Controls.Find("LblMessageTraitement", true).First().Text =
                    ListBoxXlsxPrésents.SelectedItems.Count +
                    @" classes à traiter...Veuillez patienter...";

                MessageTraitement.Controls.Find("BtnFermerMessageTraitement", true).First().Visible = false;
                MessageTraitement.ShowDialog();
            }
            else if (result == DialogResult.No)
            {
                var traitementMacro = new BackgroundWorker();
                traitementMacro.DoWork += DébutGénérerfichiersExcelDnb2;
                traitementMacro.RunWorkerCompleted += FinGénérerfichiersExcelDnb;
                traitementMacro.RunWorkerAsync();
                traitementMacro.WorkerSupportsCancellation = true;

                MessageTraitement.Controls.Find("LblMessageTraitement", true).First().Text =
                    ListBoxXlsxPrésents.SelectedItems.Count +
                    @" classes à traiter...Veuillez patienter...";

                MessageTraitement.Controls.Find("BtnFermerMessageTraitement", true).First().Visible = false;
                MessageTraitement.ShowDialog();
            }
        }

        private void DébutGénérerfichiersExcelDnb1(object sender, DoWorkEventArgs e)
        {
            GénérerFichiersXlsxDnb("Type_dnb", "DNB1-");
        }

        private void DébutGénérerfichiersExcelDnb2(object sender, DoWorkEventArgs e)
        {
            GénérerFichiersXlsxDnb("Type_dnb_oral", "DNB2-");
        }

        private void FinGénérerfichiersExcelDnb(object sender, RunWorkerCompletedEventArgs e)
        {
            EffacerListbox(ListBoxXlsxPrésents);
            VérifierCheminsDossiers();
            RemplirListeXlsxPrésents();
            LblFichiersXlsxPrésents.Text = CompterFichiersPrésents(ListBoxXlsxPrésents) + @" fichiers XLSX et " +
                                           CompterFichiersDnb(ListBoxXlsxPrésents) + @" fichiers DNB";
            MessageTraitement.Controls.Find("LblMessageTraitement", true).First().Text =
                @"Traitement des fichiers terminé !";
            MessageTraitement.Controls.Find("BtnFermerMessageTraitement", true).First().Visible = true;
            CacherFichiersXlsxDocx();
        }

        private void BtnSuppressionFichierCsvAtraiter(object sender, EventArgs e)
        {
            SuppressionFichiersIndividuels(LblCheminDossierCsv.Text, ListBoxCsvATraiter, SearchOption.TopDirectoryOnly);
        }

        private void BtnSuppressionFichierCsv_Click(object sender, EventArgs e)
        {
            SuppressionFichiersIndividuels(LblCheminDossierCsv.Text, ListBoxCsvPrésents, SearchOption.AllDirectories);
            LblFichiersCsvPrésents.Text = CompterFichiersPrésents(ListBoxCsvPrésents) + @" fichiers CSV présents";
            LblFichiersXlsxPrésents.Text = CompterFichiersPrésents(ListBoxXlsxPrésents) + @" fichiers XLSX et " +
                                           CompterFichiersDnb(ListBoxXlsxPrésents) + @" fichiers DNB";
        }

        private void BtnSuppressionFichierXlsx_Click(object sender, EventArgs e)
        {
            SuppressionFichiersIndividuels(LblCheminDossierXlsx.Text, ListBoxXlsxPrésents, SearchOption.AllDirectories);
            LblFichiersCsvPrésents.Text = CompterFichiersPrésents(ListBoxCsvPrésents) + @" fichiers CSV présents";
            LblFichiersXlsxPrésents.Text = CompterFichiersPrésents(ListBoxXlsxPrésents) + @" fichiers XLSX et " +
                                           CompterFichiersDnb(ListBoxXlsxPrésents) + @" fichiers DNB";
        }

        private void BtnSuppressionBases_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show(@"Etes-vous sûr de vouloir tout supprimer ?", @"Attention !",
                MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    SuppressionFichiersIndividuels(LblCheminDossierCsv.Text, ListBoxCsvPrésents,
                        SearchOption.AllDirectories);
                }
                catch
                {
                    // ignored
                }
                try
                {
                    SuppressionFichiersIndividuels(LblCheminDossierXlsx.Text, ListBoxXlsxPrésents,
                        SearchOption.AllDirectories);
                }
                catch
                {
                    // ignored
                }
                try
                {
                    Directory.Delete(
                        File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(1).Take(1).First(), true);
                }
                catch
                {
                    // ignored
                }
                try
                {
                    Directory.Delete(
                        File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_out.txt").Skip(1).Take(1).First(), true);
                }
                catch (Exception)
                {
                    // ignored
                }
                try
                {
                    Directory.Delete(CheminElyco + @"\ELyco\Config", true);
                }
                catch (Exception)
                {
                    // ignored
                }

                LblCheminDossierCsv.Text = "";
                LblCheminDossierXlsx.Text = "";
                ComboAnnéeScolaire.Text = "";
                ComboNiveau6.Items.Clear();
                ResetComboNiveau();

                EffacerListbox(ListBoxCsvATraiter);
                EffacerListbox(ListBoxCsvPrésents);
                EffacerListbox(ListBoxXlsxPrésents);
                RemplirListeCsvPrésents();
                RemplirListeXlsxPrésents();
                ListBoxCsvATraiter.Refresh();
                ListBoxCsvPrésents.Refresh();
                ListBoxXlsxPrésents.Refresh();
                LblFichiersCsvATraiter.Text = "";
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        private void BtnSauvegarderBases_Click(object sender, EventArgs e)
        {
            var date = DateTime.Now.ToString("dd-MM-yyyy_HH'h'mm");
            var dossierDest = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\ELyco\Backup\";
            var cheminCsv = File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(1).Take(1).First();
            var cheminCompétences =
                File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_out.txt").Skip(1).Take(1).First();

            if (!Directory.Exists(dossierDest + date))
            {
                Directory.CreateDirectory(dossierDest + date);
                ZipFile.CreateFromDirectory(CheminElyco + @"\ELyco\Config", dossierDest + date + @"\ELyco.zip");
                ZipFile.CreateFromDirectory(cheminCsv, dossierDest + date + @"\ELyco_CSV.zip");
                ZipFile.CreateFromDirectory(cheminCompétences, dossierDest + date + @"\ELyco_Competences.zip");
            }

            MessageBox.Show(@"Sauvegarde effectuée avec succès vers " + dossierDest + date);
        }

        private void BtnRestaurerBases_Click(object sender, EventArgs e)
        {
            if (File.Exists(CheminElyco + @"\ELyco\Config\ELyco_in.txt"))
                SuppressionFichiersSauvegarde(File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(1)
                    .Take(1)
                    .First());
            if (File.Exists(CheminElyco + @"\ELyco\Config\ELyco_in.txt"))
                SuppressionFichiersSauvegarde(File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_out.txt").Skip(1)
                    .Take(1)
                    .First());
            SuppressionFichiersSauvegarde(CheminElyco + @"\ELyco\Config");

            var dlg = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.ApplicationData,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\ELyco\Backup\"
            };
            SendKeys.Send("{TAB}{TAB}{RIGHT}");

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var sauvegardeSélectionnée = dlg.SelectedPath;
                ZipFile.ExtractToDirectory(sauvegardeSélectionnée + @"\ELyco.zip", CheminElyco + @"\ELyco\Config");
                ZipFile.ExtractToDirectory(sauvegardeSélectionnée + @"\ELyco_CSV.zip",
                    File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(1).Take(1).First());
                ZipFile.ExtractToDirectory(sauvegardeSélectionnée + @"\ELyco_Competences.zip",
                    File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_out.txt").Skip(1).Take(1).First());
            }

            OuvertureLogiciel(sender, e);
            CacherFichiersXlsxDocx();
            MessageBox.Show(@"Restauration effectuée avec succès depuis " + dlg.SelectedPath);
        }

        private void BtnSuppressionAnnée_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show(
                @"Etes-vous sûr de vouloir supprimer l'année " + ComboAnnéeScolaire.SelectedItem + @" ?",
                @"Attention !",
                MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    SuppressionFichiersIndividuels(LblCheminDossierCsv.Text, ListBoxCsvPrésents,
                        SearchOption.AllDirectories);
                }
                catch
                {
                    // ignored
                }
                try
                {
                    SuppressionFichiersIndividuels(LblCheminDossierXlsx.Text, ListBoxXlsxPrésents,
                        SearchOption.AllDirectories);
                }
                catch
                {
                    // ignored
                }
                try
                {
                    Directory.Delete(
                        File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(0).Take(1).First(), true);
                }
                catch
                {
                    // ignored
                }
                try
                {
                    Directory.Delete(
                        File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_out.txt").Skip(0).Take(1).First(), true);
                }
                catch (Exception)
                {
                    // ignored
                }

                LblCheminDossierCsv.Text = "";
                LblCheminDossierXlsx.Text = "";
                ComboAnnéeScolaire.Text = "";
                ResetComboNiveau();

                EffacerListbox(ListBoxCsvATraiter);
                EffacerListbox(ListBoxCsvPrésents);
                EffacerListbox(ListBoxXlsxPrésents);
                RemplirListeCsvPrésents();
                RemplirListeXlsxPrésents();
                ListBoxCsvATraiter.Refresh();
                ListBoxCsvPrésents.Refresh();
                ListBoxXlsxPrésents.Refresh();
                LblFichiersCsvATraiter.Text = "";
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        private void SuppressionSélectionsListbox(object sender, EventArgs e)
        {
            ListBoxCsvATraiter.ClearSelected();
            ListBoxCsvPrésents.ClearSelected();
            ListBoxXlsxPrésents.ClearSelected();
        }

        private void BtnStatistiques_Click(object sender, EventArgs e)
        {
            var fichiersDnbXlsx = Directory.GetFiles(LblCheminDossierXlsx.Text + @"DNB\");
            var fichierStat = LblCheminDossierXlsx.Text + @"DNB\Statistiques.xlsx";
            if (File.Exists(fichierStat)) File.Delete(fichierStat);
            var assembly = Assembly.GetExecutingAssembly();
            var input = assembly.GetManifestResourceStream("Compétences.Resources.Statistiques.xlsx");
            var output = File.Open(fichierStat, FileMode.CreateNew);
            CopieFichiersTypeDnb(input, output);
            input?.Dispose();
            output.Dispose();

            var excelApplication = new Microsoft.Office.Interop.Excel.Application();
            var statXlsx = excelApplication.Workbooks.Open(fichierStat);
            var statSynthèse = (Worksheet)statXlsx.Sheets.Item[1];
            var statMoyennes = (Worksheet)statXlsx.Sheets.Item[2];
            var statMoyennesControle = (Worksheet)statXlsx.Sheets.Item[3];

            #region Dnb1Synthèse
            int ligne = 3;
            statSynthèse.Range["B4:G13"].Value = 0;
            statMoyennes.Range["B4:I13"].Value = 0;

            foreach (var file in fichiersDnbXlsx)
            {
                var fichierDnbXlsx = Path.GetFileName(file);
                if (fichierDnbXlsx.Contains("DNB1") && fichierDnbXlsx.Contains("xlsx"))
                {
                    ligne++;
                    var fichierDnb = LblCheminDossierXlsx.Text + @"DNB\" + fichierDnbXlsx;
                    var dnbXlsx = excelApplication.Workbooks.Open(fichierDnb);
                    var dnbRécapitulatif = (Worksheet)dnbXlsx.Sheets.Item[1];

                    var range = dnbRécapitulatif.Range["AG2:AG50"];
                    var colMoyennes = dnbRécapitulatif.Range["AF2:AF50"];

                    statSynthèse.Range["A" + ligne].Value = dnbRécapitulatif.Range["B2"].Value.ToString();

                    foreach (Range element in range.Cells)
                    {
                        if (element.Value2 != null)
                        {
                            statSynthèse.Range["B" + ligne].Value =
                                    int.Parse(statSynthèse.Range["B" + ligne].Value.ToString()) + 1;

                            if (element.Value.ToString().Contains("Non"))
                            {
                                statSynthèse.Range["C" + ligne].Value =
                                    int.Parse(statSynthèse.Range["C" + ligne].Value.ToString()) + 1;
                            }
                            if (element.Value.ToString().Contains("sans mention"))
                            {
                                statSynthèse.Range["D" + ligne].Value =
                                    int.Parse(statSynthèse.Range["D" + ligne].Value.ToString()) + 1;
                            }
                            if (element.Value.ToString().Contains("mention AB"))
                            {
                                statSynthèse.Range["E" + ligne].Value =
                                    int.Parse(statSynthèse.Range["E" + ligne].Value.ToString()) + 1;
                            }
                            if (element.Value.ToString().Contains("mention B"))
                            {
                                statSynthèse.Range["F" + ligne].Value =
                                    int.Parse(statSynthèse.Range["F" + ligne].Value.ToString()) + 1;
                            }
                            if (element.Value.ToString().Contains("mention TB"))
                            {
                                statSynthèse.Range["G" + ligne].Value =
                                    int.Parse(statSynthèse.Range["G" + ligne].Value.ToString()) + 1;
                            }

                            statSynthèse.Range["H" + ligne].Formula = "=SUM(D" + ligne + ":G" + ligne + ")";
                            statSynthèse.Range["I" + ligne].Formula = "=H" + ligne + "/B" + ligne;
                        }
                    }
                    float total = 0;
                    int compteur = 0;
                    foreach (Range element in colMoyennes.Cells)
                    {
                        if (element.Value2 != null)
                        {
                            total = total + float.Parse(element.Value.ToString());
                            compteur++;
                        }
                    }
                    statSynthèse.Range["J" + ligne].Value = total / compteur;
                    dnbXlsx.Close();
                }
            }
            var range1 = statSynthèse.Range["A" + (ligne + 1), "I13"];
            range1.Value = "";
            statSynthèse.Range["A" + (ligne + 2)].Value = "Niveau";

            var colonne = 'B';
            for (int i = 1; i < 8; i++)
            {
                statSynthèse.Range[colonne.ToString() + (ligne + 2)].Formula = "=SUM(" + colonne + "4:" + colonne + ligne + ")";
                colonne++;
            }

            statSynthèse.Range["I" + (ligne + 2)].Formula = "=H" + (ligne + 2) + "/B" + (ligne + 2);
            statSynthèse.Range["J" + (ligne + 2)].Formula = "=AVERAGE(J4:J" + ligne;
            #endregion

            #region Dnb2Synthèse
            ligne = 15;
            statSynthèse.Range["B16:G25"].Value = 0;
            
            foreach (var file in fichiersDnbXlsx)
            {
                var fichierDnbXlsx = Path.GetFileName(file);
                if (fichierDnbXlsx.Contains("DNB2") && fichierDnbXlsx.Contains("xlsx"))
                {
                    ligne++;
                    var fichierDnb = LblCheminDossierXlsx.Text + @"DNB\" + fichierDnbXlsx;
                    var dnbXlsx = excelApplication.Workbooks.Open(fichierDnb);
                    var dnbRécapitulatif = (Worksheet)dnbXlsx.Sheets.Item[1];
                    var colMoyennes = dnbRécapitulatif.Range["AG2:AG50"];
                    var range = dnbRécapitulatif.Range["AH2:AH50"];

                    statSynthèse.Range["A" + ligne].Value = dnbRécapitulatif.Range["B2"].Value.ToString();

                    foreach (Range element in range.Cells)
                    {
                        if (element.Value2 != null)
                        {
                            statSynthèse.Range["B" + ligne].Value =
                                   int.Parse(statSynthèse.Range["B" + ligne].Value.ToString()) + 1;

                            if (element.Value.ToString().Contains("Non"))
                            {
                                statSynthèse.Range["C" + ligne].Value =
                                    int.Parse(statSynthèse.Range["C" + ligne].Value.ToString()) + 1;
                            }
                            if (element.Value.ToString().Contains("sans mention"))
                            {
                                statSynthèse.Range["D" + ligne].Value =
                                    int.Parse(statSynthèse.Range["D" + ligne].Value.ToString()) + 1;
                            }
                            if (element.Value.ToString().Contains("mention AB"))
                            {
                                statSynthèse.Range["E" + ligne].Value =
                                    int.Parse(statSynthèse.Range["E" + ligne].Value.ToString()) + 1;
                            }
                            if (element.Value.ToString().Contains("mention B"))
                            {
                                statSynthèse.Range["F" + ligne].Value =
                                    int.Parse(statSynthèse.Range["F" + ligne].Value.ToString()) + 1;
                            }
                            if (element.Value.ToString().Contains("mention TB"))
                            {
                                statSynthèse.Range["G" + ligne].Value =
                                    int.Parse(statSynthèse.Range["G" + ligne].Value.ToString()) + 1;
                            }

                            statSynthèse.Range["H" + ligne].Formula = "=SUM(D" + ligne + ":G" + ligne + ")";
                            statSynthèse.Range["I" + ligne].Formula = "=H" + ligne + "/B" + ligne;
                            statSynthèse.Range["I" + ligne + ":I" + ligne].NumberFormat = "0,0%";
                            if (statSynthèse.Range["I" + ligne].Text == "0,0%")
                            {
                                statSynthèse.Range["B" + ligne + ":I" + ligne].NumberFormat = ";;;";
                            }
                        }
                    }
                    float total = 0;
                    int compteur = 0;
                    foreach (Range element in colMoyennes.Cells)
                    {
                        if (element.Value2 != null)
                        {
                            total = total + float.Parse(element.Value.ToString());
                            compteur++;
                        }
                    }
                    statSynthèse.Range["J" + ligne].Value = total / compteur;
                    if (statSynthèse.Range["B" + ligne + ":I" + ligne].NumberFormat.ToString() == ";;;")
                    {
                        statSynthèse.Range["J" + ligne].NumberFormat = ";;;";
                    }
                    dnbXlsx.Close();
                }
            }
            var range2 = statSynthèse.Range["A" + (ligne + 1), "I25"];
            range2.Value = "";

            statSynthèse.Range["A1"].Value = "Année scolaire " + File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(2).Take(3).First();
            statSynthèse.Range["A" + (ligne + 2)].Value = "Niveau";

            colonne = 'B';
            for (int i = 1; i < 8; i++)
            {
                statSynthèse.Range[colonne.ToString() + (ligne + 2)].Formula = "=SUM(" + colonne + "16:" + colonne + ligne + ")";
                colonne++;
            }

            statSynthèse.Range["I" + (ligne + 2)].Formula = "=H" + (ligne + 2) + "/B" + (ligne + 2);
            statSynthèse.Range["J" + (ligne + 2)].Formula = "=AVERAGE(J16:J" + ligne + ")";
            if (statSynthèse.Range["I" + (ligne + 2)].Text == "0,0%")
            {
                statSynthèse.Range["B" + (ligne + 2) + ":I" + (ligne + 2)].NumberFormat = ";;;";
                statSynthèse.Range["J" + (ligne + 2)].NumberFormat = ";;;";
            }
            #endregion

            #region Dnb1MoyennesEpreuves
            ligne = 3;

            foreach (var file in fichiersDnbXlsx)
            {
                var fichierDnbXlsx = Path.GetFileName(file);

                if (fichierDnbXlsx.Contains("DNB1") && fichierDnbXlsx.Contains("xlsx"))
                {
                    ligne++;
                    var fichierDnb = LblCheminDossierXlsx.Text + @"DNB\" + fichierDnbXlsx;
                    var dnbXlsx = excelApplication.Workbooks.Open(fichierDnb);
                    var dnbEpreuvesEcrites = (Worksheet)dnbXlsx.Sheets.Item[2];
                    var dnbRécapitulatif = (Worksheet)dnbXlsx.Sheets.Item[1];
                    statMoyennes.Range["A" + ligne].Value = ((Worksheet)dnbXlsx.Sheets.Item[1]).Range["B2"].Value.ToString();
                    statMoyennes.Range["I" + ligne].Value = "";
                    int effectif = int.Parse(statSynthèse.Range["B" + ligne.ToString()].Value.ToString());
                    colonne = 'B';
                    for (int i = 1; i < 8; i++)
                    {
                        int barême = int.Parse(dnbEpreuvesEcrites.Range[colonne + "1"].Value.ToString().Split(new[] { '/', ')' })[1]);

                        dnbEpreuvesEcrites.Range["K" + (ligne)].Formula = "=SUM(" + colonne + "2:" + colonne + "40)";
                        dnbEpreuvesEcrites.Range["K" + (ligne)].Value =
                            float.Parse(dnbEpreuvesEcrites.Range["K" + (ligne)].Value.ToString()) / effectif;

                        statMoyennes.Range[colonne.ToString() + ligne].Value =
                            float.Parse(dnbEpreuvesEcrites.Range["K" + (ligne)].Value.ToString()) / barême * 20;

                        colonne++;
                    }
                    dnbRécapitulatif.Range["L" + (ligne)].Formula = "=AVERAGE(AD2:AD" + (effectif + 1) + ")";
                    statMoyennes.Range["J" + ligne].Value = float.Parse(dnbRécapitulatif.Range["L" + (ligne)].Value.ToString());
                   }

            }

            var range3 = statMoyennes.Range["A" + (ligne + 1), "J13"];
            range3.Value = "";

            statMoyennes.Range["A1"].Value = "Année scolaire " + File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(2).Take(3).First();
            statMoyennes.Range["A" + (ligne + 2)].Value = "Niveau";

            colonne = 'B';
            for (int i = 1; i < 8; i++)
            {
                statMoyennes.Range[colonne.ToString() + (ligne + 2)].Formula = "=AVERAGE(" + colonne + "4:" + colonne + ligne + ")";
                colonne++;
            }
            statMoyennes.Range["J" + (ligne + 2)].Formula = "=AVERAGE(J4:J" + ligne + ")";

            #endregion

            #region Dnb2MoyennesEpreuves
            ligne = 15;

            foreach (var file in fichiersDnbXlsx)
            {
                var fichierDnbXlsx = Path.GetFileName(file);

                if (fichierDnbXlsx.Contains("DNB2") && fichierDnbXlsx.Contains("xlsx"))
                {
                    ligne++;
                    var fichierDnb = LblCheminDossierXlsx.Text + @"DNB\" + fichierDnbXlsx;
                    var dnbXlsx = excelApplication.Workbooks.Open(fichierDnb);
                    var dnbEpreuvesEcrites = (Worksheet)dnbXlsx.Sheets.Item[2];
                    var dnbRécapitulatif = (Worksheet)dnbXlsx.Sheets.Item[1];
                    statMoyennes.Range["A" + ligne].Value = ((Worksheet)dnbXlsx.Sheets.Item[1]).Range["B2"].Value.ToString(); //classe
                    int effectif = int.Parse(statSynthèse.Range["B" + (ligne - 12)].Value.ToString());
                    colonne = 'B';
                    for (int i = 1; i < 9; i++)
                    {
                        int barême = int.Parse(dnbEpreuvesEcrites.Range[colonne + "1"].Value.ToString().Split(new[] { '/', ')' })[1]);
                        dnbEpreuvesEcrites.Range["K" + (ligne)].Formula = "=SUM(" + colonne + "2:" + colonne + "40)";
                        dnbEpreuvesEcrites.Range["K" + (ligne)].Value =
                            float.Parse(dnbEpreuvesEcrites.Range["K" + (ligne)].Value.ToString()) / effectif;

                        statMoyennes.Range[colonne.ToString() + ligne].Value =
                            float.Parse(dnbEpreuvesEcrites.Range["K" + (ligne)].Value.ToString()) / barême * 20;
                        if (statMoyennes.Range[colonne.ToString() + ligne].Text == "0,0")
                            statMoyennes.Range[colonne.ToString() + ligne].NumberFormat = ";;;";

                        colonne++;
                    }

                    dnbRécapitulatif.Range["L" + (ligne)].Formula = "=AVERAGE(AE2:AE" + (effectif + 1) + ")";
                    statMoyennes.Range["J" + ligne].Value = float.Parse(dnbRécapitulatif.Range["L" + (ligne)].Value.ToString());
                    if (statMoyennes.Range["J" + ligne].Text == "0,0")
                    {
                        statMoyennes.Range["J" + ligne].NumberFormat = ";;;";
                    }
                    excelApplication.DisplayAlerts = false;
                    dnbXlsx.Close();
                }
            }

            var range4 = statMoyennes.Range["A" + (ligne + 1), "J25"];
            range4.Value = "";

            statMoyennes.Range["A14"].Value = "DNB N°2 - Moyennes des épreuves écrites et de l'oral  ( /20)";
            statMoyennes.Range["A" + (ligne + 2)].Value = "Niveau";

            colonne = 'B';
            for (int i = 1; i < 9; i++)
            {
                statMoyennes.Range[colonne.ToString() + (ligne + 2)].Formula = "=AVERAGE(" + colonne + "16:" + colonne + ligne + ")";
                if (statMoyennes.Range[colonne.ToString() + (ligne + 2)].Text == "0,0")
                {
                    statMoyennes.Range[colonne.ToString() + (ligne + 2)].NumberFormat = ";;;";
                }
                colonne++;
            }
            statMoyennes.Range["J" + (ligne + 2)].Formula = "=AVERAGE(J16:J" + ligne + ")";
            if (statMoyennes.Range["J" + (ligne + 2)].Text == "0,0")
            {
                statMoyennes.Range["J" + (ligne + 2)].NumberFormat = ";;;";
            }
            #endregion

            #region Dnb1MoyennesControleContinu
            ligne = 3;
            
            foreach (var file in fichiersDnbXlsx)
            {
                var fichierDnbXlsx = Path.GetFileName(file);

                if (fichierDnbXlsx.Contains("DNB1") && fichierDnbXlsx.Contains("xlsx"))
                {
                    ligne++;
                    var colonne1 = 'C';
                    var fichierDnb = LblCheminDossierXlsx.Text + @"DNB\" + fichierDnbXlsx;
                    var dnbXlsx = excelApplication.Workbooks.Open(fichierDnb);
                    //var dnbEpreuvesEcrites = (Worksheet)dnbXlsx.Sheets.Item[2];
                    var dnbRécapitulatif = (Worksheet)dnbXlsx.Sheets.Item[1];
                    statMoyennesControle.Range["A" + ligne].Value = ((Worksheet)dnbXlsx.Sheets.Item[1]).Range["B2"].Value.ToString();
                    statMoyennesControle.Range["I" + ligne].Value = "";
                    int effectif = int.Parse(statSynthèse.Range["B" + ligne.ToString()].Value.ToString());
                    colonne = 'B';
                    for (int i = 1; i < 9; i++)
                    {
                        float somme = 0;
                        for (int j = 2; j <= effectif + 1; j++)
                        {
                            somme = somme + float.Parse(dnbRécapitulatif.Range[colonne1.ToString() + j].Value.ToString());

                        }

                        statMoyennesControle.Range[colonne.ToString() + ligne].Value = somme / effectif;

                        colonne++;
                        colonne1++;
                    }

                    statMoyennesControle.Range["J" + ligne].Formula = "=AVERAGE(B" + ligne + ":I" + ligne;
                    dnbXlsx.Close();
                }
            }

            var range5 = statMoyennesControle.Range["A" + (ligne + 1), "J13"];
            range5.Value = "";

            statMoyennesControle.Range["A1"].Value = "Année scolaire " + File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(2).Take(3).First();
            statMoyennesControle.Range["A" + (ligne + 2)].Value = "Niveau";

            colonne = 'B';
            for (int i = 1; i < 9; i++)
            {
                statMoyennesControle.Range[colonne.ToString() + (ligne + 2)].Formula = "=AVERAGE(" + colonne + "4:" + colonne + ligne + ")";
                colonne++;
            }
            statMoyennesControle.Range["J" + (ligne + 2)].Formula = "=AVERAGE(J4:J" + ligne + ")";
            #endregion

            #region Dnb2MoyennesControleContinu
            ligne = 15;

            foreach (var file in fichiersDnbXlsx)
            {
                var fichierDnbXlsx = Path.GetFileName(file);

                if (fichierDnbXlsx.Contains("DNB2") && fichierDnbXlsx.Contains("xlsx"))
                {
                    ligne++;
                    var colonne1 = 'C';
                    var fichierDnb = LblCheminDossierXlsx.Text + @"DNB\" + fichierDnbXlsx;
                    var dnbXlsx = excelApplication.Workbooks.Open(fichierDnb);
                    //var dnbEpreuvesEcrites = (Worksheet)dnbXlsx.Sheets.Item[2];
                    var dnbRécapitulatif = (Worksheet)dnbXlsx.Sheets.Item[1];
                    statMoyennesControle.Range["A" + ligne].Value = ((Worksheet)dnbXlsx.Sheets.Item[1]).Range["B2"].Value.ToString();
                    statMoyennesControle.Range["I" + ligne].Value = "";
                    int effectif = int.Parse(statSynthèse.Range["B" + ligne.ToString()].Value.ToString());
                    colonne = 'B';
                    for (int i = 1; i < 9; i++)
                    {
                        float somme = 0;
                        for (int j = 2; j <= effectif + 1; j++)
                        {
                            somme = somme + float.Parse(dnbRécapitulatif.Range[colonne1.ToString() + j].Value.ToString());

                        }

                        statMoyennesControle.Range[colonne.ToString() + ligne].Value = somme / effectif;

                        colonne++;
                        colonne1++;
                    }

                    statMoyennesControle.Range["J" + ligne].Formula = "=AVERAGE(B" + ligne + ":I" + ligne;
                    dnbXlsx.Close();
                }
            }

            var range6 = statMoyennesControle.Range["A" + (ligne + 1), "J25"];
            range6.Value = "";

            statMoyennesControle.Range["A1"].Value = "Année scolaire " + File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(2).Take(3).First();
            statMoyennesControle.Range["A" + (ligne + 2)].Value = "Niveau";

            colonne = 'B';
            for (int i = 1; i < 9; i++)
            {
                statMoyennesControle.Range[colonne.ToString() + (ligne + 2)].Formula = "=AVERAGE(" + colonne + "4:" + colonne + ligne + ")";
                colonne++;
            }
            statMoyennesControle.Range["J" + (ligne + 2)].Formula = "=AVERAGE(J16:J" + ligne + ")";
            #endregion


            excelApplication.DisplayAlerts = false;
            statXlsx.SaveAs(fichierStat);
            statXlsx.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF,
                            LblCheminDossierXlsx.Text + @"DNB\Statistiques.pdf");
            statXlsx.Close();
            GC.Collect();
            CacherFichiersXlsxDocx();
        }

        private void OuvrirFichierXlsxDocx(object sender, MouseEventArgs e)
        {
            var fichierP1 = LblCheminDossierXlsx.Text + @"1ère période\" + ListBoxXlsxPrésents.SelectedItem;
            var fichierP2 = LblCheminDossierXlsx.Text + @"2ème période\" + ListBoxXlsxPrésents.SelectedItem;
            var fichierP3 = LblCheminDossierXlsx.Text + @"3ème période\" + ListBoxXlsxPrésents.SelectedItem;
            var fichierAnnee = LblCheminDossierXlsx.Text + @"Année\" + ListBoxXlsxPrésents.SelectedItem;
            var fichierDnb = LblCheminDossierXlsx.Text + @"DNB\" + ListBoxXlsxPrésents.SelectedItem;
            if (File.Exists(fichierP1))
                Process.Start(fichierP1);
            if (File.Exists(fichierP2))
                Process.Start(fichierP2);
            if (File.Exists(fichierP3))
                Process.Start(fichierP3);
            if (File.Exists(fichierAnnee))
                Process.Start(fichierAnnee);
            if (File.Exists(fichierDnb))
                Process.Start(fichierDnb);
        }

        private void SélectionFichierCsvATraiter(object sender, EventArgs e)
        {
            BtnSuppressionFichierCsvATraiter.Enabled = ListBoxCsvATraiter.SelectedItems.Count != 0;
        }

        private void SélectionFichierCsvPrésent(object sender, EventArgs e)
        {
            File.WriteAllText(CheminElyco + @"\ELyco\Config\ELyco_classes_annee.txt", string.Empty);
            foreach (var listBoxItem in ListBoxCsvPrésents.SelectedItems)
                if (listBoxItem.ToString().Contains("competence"))
                {
                    File.AppendAllText(CheminElyco + @"\ELyco\Config\ELyco_classes_annee.txt",
                        Path.GetFileName(listBoxItem.ToString()).Substring(25, 2) + Environment.NewLine);
                    File.AppendAllText(CheminElyco + @"\ELyco\Config\ELyco_classes.txt",
                        Path.GetFileName(listBoxItem.ToString()).Substring(25, 2) + Environment.NewLine);
                    BtnSuppressionFichierCsv.Enabled = true;
                }
            SélectionPériode(new object(), new EventArgs());
            if (ListBoxCsvPrésents.SelectedItems.Count != 0 &&
                ListBoxCsvPrésents.SelectedItem.ToString().Contains("competence"))
                BtnSuppressionFichierCsv.Enabled = true;
            else BtnSuppressionFichierCsv.Enabled = false;
        }

        private void SélectionFichierXlsxDocxPrésent(object sender, EventArgs e)
        {
            File.WriteAllText(CheminElyco + @"\ELyco\Config\ELyco_classes_dnb.txt", string.Empty);
            var listeDnbXlsx = new ListBox();
            var listeDocxXlsx = new ListBox();
            var listeAnnéeXlsx = new ListBox();
            var listeSélection = new ListBox();
            foreach (var listBoxItem in ListBoxXlsxPrésents.SelectedItems)
            {
                listeSélection.Items.Add(listBoxItem.ToString());
                if (listBoxItem.ToString().Contains("DNB") && listBoxItem.ToString().Contains("xlsx"))
                {
                    File.AppendAllText(CheminElyco + @"\ELyco\Config\ELyco_classes_dnb.txt",
                        Path.GetFileName(listBoxItem.ToString()).Substring(0, 18) + Environment.NewLine);
                    listeDnbXlsx.Items.Add(listBoxItem.ToString());
                }
                if (listBoxItem.ToString().Contains("docx") || listBoxItem.ToString().Contains("xlsx") ||
                    listBoxItem.ToString().Contains("pdf"))
                    listeDocxXlsx.Items.Add(listBoxItem.ToString());
                if (listBoxItem.ToString().Contains("Annee") && listBoxItem.ToString().Contains("3"))
                    listeAnnéeXlsx.Items.Add(listBoxItem.ToString());
            }

            foreach (var item in listeSélection.Items)
                if (listeDnbXlsx.Items.Contains(item))
                {
                    BtnGénérerPublipostageDnb.Enabled = true;
                }
                else
                {
                    BtnGénérerPublipostageDnb.Enabled = false;
                    break;
                }

            foreach (var item in listeSélection.Items)
                if (listeDocxXlsx.Items.Contains(item))
                {
                    BtnSuppressionFichierXlsx.Enabled = true;
                }
                else
                {
                    BtnSuppressionFichierXlsx.Enabled = false;
                    break;
                }
            foreach (var item in listeSélection.Items)
                if (listeAnnéeXlsx.Items.Contains(item))
                {
                    BtnGénérerfichiersExcelDnb.Enabled = true;
                }
                else
                {
                    BtnGénérerfichiersExcelDnb.Enabled = false;
                    break;
                }

            if (ListBoxXlsxPrésents.SelectedItems.Count == 0)

            {
                BtnGénérerfichiersExcelDnb.Enabled = false;
                BtnGénérerPublipostageDnb.Enabled = false;
                BtnSuppressionFichierXlsx.Enabled = false;
            }
        }

        private void SélectionPériode(object sender, EventArgs e)
        {
            DétectionPériode();
            if (DétectionPériode() != null && DétectionPériode().Contains("période") &&
                ListBoxCsvATraiter.Items.Count != 0 || DétectionPériode() != null &&
                ListBoxCsvPrésents.SelectedItems.Count != 0 &&
                ListBoxCsvPrésents.SelectedItem.ToString().Contains("competence"))
                BtnLancerTraitement.Enabled = true;
            else BtnLancerTraitement.Enabled = false;
        }

        private void ComboAnnéeScolaire_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboAnnéeScolaire.SelectedItem.ToString() != "")
                if (File.Exists(CheminElyco + @"\ELyco\Config\ELyco_in.txt"))
                {
                    var cheminCsv =
                        File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(1).Take(2).First() + @"\" +
                        ComboAnnéeScolaire.SelectedItem;
                    var cheminXlsx =
                        File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_out.txt").Skip(1).Take(2).First() + @"\" +
                        ComboAnnéeScolaire.SelectedItem;

                    if (Directory.Exists(cheminCsv) && Directory.Exists(cheminXlsx))
                    {
                        ChangerLigneFichierTxt(
                            File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(1).Take(2).First() + @"\" +
                            ComboAnnéeScolaire.SelectedItem, CheminElyco + @"\ELyco\Config\ELyco_in.txt", 1);
                        ChangerLigneFichierTxt(ComboAnnéeScolaire.SelectedItem.ToString(),
                            CheminElyco + @"\ELyco\Config\ELyco_in.txt", 3);
                        ChangerLigneFichierTxt(
                            File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_out.txt").Skip(1).Take(2).First() +
                            @"\" + ComboAnnéeScolaire.SelectedItem, CheminElyco + @"\ELyco\Config\ELyco_out.txt", 1);

                        LblCheminDossierCsv.Text =
                            File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(1).Take(2).First() + @"\" +
                            ComboAnnéeScolaire.SelectedItem + @"\";
                        LblCheminDossierXlsx.Text =
                            File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_out.txt").Skip(1).Take(2).First() +
                            @"\" +
                            ComboAnnéeScolaire.SelectedItem + @"\";

                        var chemin = new DirectoryInfo(LblCheminDossierCsv.Text + @"\Année");
                        var dossiers = chemin.GetDirectories();
                        var nb6 = 0;
                        var nb5 = 0;
                        var nb4 = 0;
                        var nb3 = 0;
                        foreach (var dossier in dossiers)
                        {
                            if (dossier.ToString().Contains("6"))
                            {
                                ComboNiveau6.Items.Add(dossier);
                                nb6++;
                            }
                            if (dossier.ToString().Contains("5"))
                            {
                                ComboNiveau5.Items.Add(dossier);
                                nb5++;
                            }
                            if (dossier.ToString().Contains("4"))
                            {
                                ComboNiveau4.Items.Add(dossier);
                                nb4++;
                            }
                            if (dossier.ToString().Contains("3"))
                            {
                                ComboNiveau3.Items.Add(dossier);
                                nb3++;
                            }
                        }

                        ChangerLigneFichierTxt(nb6.ToString(), CheminElyco + @"\ELyco\Config\ELyco_in.txt", 4);
                        ChangerLigneFichierTxt(nb5.ToString(), CheminElyco + @"\ELyco\Config\ELyco_in.txt", 5);
                        ChangerLigneFichierTxt(nb4.ToString(), CheminElyco + @"\ELyco\Config\ELyco_in.txt", 6);
                        ChangerLigneFichierTxt(nb3.ToString(), CheminElyco + @"\ELyco\Config\ELyco_in.txt", 7);

                        var ligne = 7;
                        foreach (ComboBox combo in PanelClasses.Controls)
                        {
                            var niveau = combo.Name.Substring(combo.Name.Length - 1);

                            combo.Items.Clear();
                            combo.Items.Add(File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(ligne - 1)
                                                .Take(ligne).First() + " classes");

                            foreach (var dossier in dossiers)
                                if (dossier.ToString().Contains(niveau)) combo.Items.Add(dossier);
                            combo.Items.Add("Masquer niveau");
                            ligne--;
                        }
                        OuvertureLogiciel(sender, e);
                    }
                    else
                    {
                        ChangerLigneFichierTxt(
                            File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(1).Take(2).First() + @"\" +
                            ComboAnnéeScolaire.SelectedItem, CheminElyco + @"\ELyco\Config\ELyco_in.txt", 1);
                        ChangerLigneFichierTxt(ComboAnnéeScolaire.SelectedItem.ToString(),
                            CheminElyco + @"\ELyco\Config\ELyco_in.txt", 3);
                        ChangerLigneFichierTxt(
                            File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_out.txt").Skip(1).Take(2).First() +
                            @"\" + ComboAnnéeScolaire.SelectedItem, CheminElyco + @"\ELyco\Config\ELyco_out.txt", 1);

                        LblCheminDossierCsv.Text =
                            File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_in.txt").Skip(1).Take(2).First() + @"\" +
                            ComboAnnéeScolaire.SelectedItem + @"\";
                        LblCheminDossierXlsx.Text =
                            File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_out.txt").Skip(1).Take(2).First() +
                            @"\" +
                            ComboAnnéeScolaire.SelectedItem + @"\";
                        ResetComboNiveau();
                    }
                }
        }

        private void ChangementFiltres(object sender, EventArgs e)
        {
            EffacerListbox(ListBoxCsvATraiter);
            EffacerListbox(ListBoxCsvPrésents);
            EffacerListbox(ListBoxXlsxPrésents);
            RemplirListeCsvPrésents();
            RemplirListeXlsxPrésents();
            ListBoxCsvATraiter.Refresh();
            ListBoxCsvPrésents.Refresh();
            ListBoxXlsxPrésents.Refresh();
            LblFichiersCsvATraiter.Text = "";
            LblFichiersCsvPrésents.Text = "";
            LblFichiersCsvPrésents.Text = CompterFichiersPrésents(ListBoxCsvPrésents) + @" fichiers CSV";
            LblFichiersXlsxPrésents.Text = "";
            LblFichiersXlsxPrésents.Text = CompterFichiersPrésents(ListBoxXlsxPrésents) + @" fichiers XLSX et " +
                                           CompterFichiersDnb(ListBoxXlsxPrésents) + @" fichiers DNB";
        }

        private void GlisserDéplacerCsvAtraiter(object sender, DragEventArgs e)
        {
            var fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (var file in fileList)
            {
                var filename = Path.GetFullPath(file);
                ListBoxCsvATraiter.Items.Add(filename);
            }

            File.WriteAllText(CheminElyco + @"\ELyco\Config\ELyco_classes.txt", string.Empty);

            foreach (var listBoxItem in ListBoxCsvATraiter.Items)
            {
                if (!File.Exists(LblCheminDossierCsv.Text + @"\" + Path.GetFileName(listBoxItem.ToString())))
                    File.Copy(listBoxItem.ToString(),
                        LblCheminDossierCsv.Text + @"\" + Path.GetFileName(listBoxItem.ToString()));

                File.AppendAllText(CheminElyco + @"\ELyco\Config\ELyco_classes.txt",
                    Path.GetFileName(listBoxItem.ToString()).Substring(25, 2) + Environment.NewLine);
            }

            RafraichirListbox();
        }

        private void GlisserValiderCsvAtraiter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.All;
        }

        private void SuppressionFichiersIndividuels(string chemin, ListBox liste, SearchOption chercher)
        {
            var files = Directory.GetFiles(chemin, "*.*", chercher);

            foreach (var file in files)
                foreach (var item in liste.SelectedItems)
                    if (file.Contains(item.ToString()) &&
                        (file.Contains("competence") || file.Contains("DNB1-") || file.Contains("DNB2-")))
                        File.Delete(file);
            var selectedItems = liste.SelectedItems;

            if (liste.SelectedIndex != -1)
                for (var i = selectedItems.Count - 1; i >= 0; i--)

                    if (selectedItems.Contains("competence") || selectedItems.Contains("DNB1-") ||
                        selectedItems.Contains("DNB2-"))
                        liste.Items.Remove(selectedItems[i]);
            RafraichirListbox();
        }

        private void SuppressionFichiersSauvegarde(string chemin)
        {
            var dossier = new DirectoryInfo(chemin);

            if (Directory.Exists(dossier.ToString()))
            {
                foreach (var file in dossier.GetFiles())
                    file.Delete();
                foreach (var dir in dossier.GetDirectories())
                    dir.Delete(true);
            }
        }

        private void EffacerListbox(ListBox liste)
        {
            for (var i = liste.Items.Count - 1; i >= 0; i--)
                liste.Items.RemoveAt(i);
        }

        private void CréationArborescence(string niveau)
        {
            var classe = 'A';

            var combo = (ComboBox)PanelClasses.Controls.Find(string.Format("ComboNiveau" + niveau), false)
                .FirstOrDefault();
            if (combo != null)
                for (var i = 1; i <= int.Parse(combo.Items[combo.SelectedIndex].ToString()); i++)
                {
                    Directory.CreateDirectory(LblCheminDossierCsv.Text + @"\" + "1ère période" + @"\" + niveau +
                                              classe);
                    Directory.CreateDirectory(LblCheminDossierCsv.Text + @"\" + "2ème période" + @"\" + niveau +
                                              classe);
                    Directory.CreateDirectory(LblCheminDossierCsv.Text + @"\" + "3ème période" + @"\" + niveau +
                                              classe);
                    Directory.CreateDirectory(LblCheminDossierCsv.Text + @"\" + "Année" + @"\" + niveau + classe);
                    Directory.CreateDirectory(LblCheminDossierCsv.Text + @"\" + niveau + classe);
                    classe++; // c1 is 'B' now
                }
        }

        private void VérifierCheminsDossiers()
        {
            if (File.Exists(CheminElyco + @"\ELyco\Config\ELyco_in.txt"))
                using (TextReader tr = new StreamReader(CheminElyco + @"\ELyco\Config\ELyco_in.txt"))
                {
                    LblCheminDossierCsv.Text = tr.ReadLine() + @"\";
                }
            if (File.Exists(CheminElyco + @"\ELyco\Config\ELyco_out.txt"))
                using (TextReader tr1 = new StreamReader(CheminElyco + @"\ELyco\Config\ELyco_out.txt"))
                {
                    LblCheminDossierXlsx.Text = tr1.ReadLine() + @"\";
                }
        }

        private void VérifierDoublonClasseCsv(string période)
        {
            var templist = new ListBox();
            foreach (var t in ListBoxCsvATraiter.Items)

            {
                var classe = Path.GetFileName(t.ToString()).Substring(25, 2);

                foreach (var s in Directory
                    .GetFiles(LblCheminDossierCsv.Text + période + @"\" + classe + @"\", "*.csv")
                    .Select(Path.GetFileName))
                {
                    var classe1 = s.Substring(25, 2);

                    if (classe == classe1)
                    {
                        MessageBox.Show(@"Il existe déjà un fichier pour la classe " + classe1 + @" dans le dossier '" +
                                        période + @"'");
                        templist.Items.Add(t);
                        File.Delete(LblCheminDossierCsv.Text + t);
                    }
                }
            }

            foreach (var v in templist.Items)
                ListBoxCsvATraiter.Items.Remove(v);

            EffacerListbox(ListBoxCsvATraiter);
            VérifierCheminsDossiers();
            RemplirListeCsvATraiter();
            LblFichiersCsvATraiter.Text = ListBoxCsvATraiter.Items.Count + @" classes à traiter";
        }

        private static void ChangerLigneFichierTxt(string newText, string fileName, int lineToEdit)
        {
            var arrLine = File.ReadAllLines(fileName);
            arrLine[lineToEdit - 1] = newText;
            File.WriteAllLines(fileName, arrLine);
        }

        private void ListeFichiersPrésents(string directoryPath, string periode, ListBox liste)

        {
            var directoryInfo = new DirectoryInfo(directoryPath + periode);

            if (directoryInfo.Exists)

            {
                var fileInfo = directoryInfo.GetFiles();

                var subdirectoryInfo = directoryInfo.GetDirectories();

                if (liste == ListBoxCsvPrésents)
                    foreach (var subDirectory in subdirectoryInfo)

                        ListeFichiersPrésents(subDirectory.FullName, "", liste);

                foreach (var file in fileInfo)
                    foreach (CheckBox filtre in PanelFiltres.Controls)
                    {
                        if (file.Length > 2000 && file.Name.Contains(filtre.Text) &&
                            filtre.Checked)
                            liste.Items.Add(file.Name);
                        if (file.Length <= 2000 && !file.Name.Contains(".txt"))
                            file.Delete();
                        if (file.Name.Contains("Type"))
                            liste.Items.Remove(file.Name);
                    }

                foreach (var file in fileInfo)
                    foreach (ComboBox filtre2 in PanelClasses.Controls)
                        foreach (var item in filtre2.Items)
                            if (filtre2.SelectedItem != null)
                                if (filtre2.SelectedItem.ToString() == "Masquer niveau")
                                    if (file.Name.Contains(item.ToString()))
                                        liste.Items.Remove(file.Name);
            }
        }

        private void CopieFichiersTypeDnb(Stream input, Stream output)
        {
            var buffer = new byte[32768];
            while (true)
            {
                var read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                    return;
                output.Write(buffer, 0, read);
            }
        }

        public void GénérerFichiersXlsxDnb(string typeDnb, string nommage)
        {
            var fichiers = Directory.GetFiles(LblCheminDossierXlsx.Text + @"Année\");

            foreach (var file in fichiers)
            {
                var classe = Path.GetFileNameWithoutExtension(file).Substring(17);
                var fichier = Path.GetFileName(file);
                foreach (var fichierSélectionné in ListBoxXlsxPrésents.SelectedItems)
                    if (fichierSélectionné.ToString() == fichier)
                    {
                        var strPath = LblCheminDossierXlsx.Text + @"DNB\" + typeDnb + ".xlsx";
                        if (File.Exists(strPath)) File.Delete(strPath);
                        var assembly = Assembly.GetExecutingAssembly();
                        var input = assembly.GetManifestResourceStream("Compétences.Resources." + typeDnb + ".xlsx");
                        var output = File.Open(strPath, FileMode.CreateNew);
                        CopieFichiersTypeDnb(input, output);
                        input?.Dispose();
                        output.Dispose();

                        var strPath1 = LblCheminDossierXlsx.Text + @"DNB\" + typeDnb + ".docx";
                        if (File.Exists(strPath1)) File.Delete(strPath1);
                        var assembly1 = Assembly.GetExecutingAssembly();
                        var input1 = assembly1.GetManifestResourceStream("Compétences.Resources." + typeDnb + ".docx");
                        var output1 = File.Open(strPath1, FileMode.CreateNew);
                        CopieFichiersTypeDnb(input1, output1);
                        input1?.Dispose();
                        output1.Dispose();

                        var excelApplication = new Microsoft.Office.Interop.Excel.Application();

                        var srcPath = LblCheminDossierXlsx.Text + @"Année\" + fichier;
                        var srcworkBook = excelApplication.Workbooks.Open(srcPath);
                        var srcworkSheet = (Worksheet)srcworkBook.Sheets.Item[1];

                        var destPath = strPath;
                        var destworkBook = excelApplication.Workbooks.Open(destPath, 0, false);
                        var destworkSheet = (Worksheet)destworkBook.Sheets.Item[1];
                        var destworkSheet2 = (Worksheet)destworkBook.Sheets.Item[2];

                        var range = srcworkSheet.Range["A2:A50"];
                        var cnt = -3;

                        foreach (Range element in range.Cells)

                            if (element.Value2 != null)
                                cnt = cnt + 1;

                        var from = srcworkSheet.Range["B1:J" + (cnt + 1)]; //Copie tableau compétences
                        var to = destworkSheet.Range["AI1"]; //à modifier

                        var from1 = srcworkSheet.Range["A2:A" + (cnt + 1)]; //Copie noms vers récapitilatif
                        var to1 = destworkSheet.Range["A2"];

                        var from2 = srcworkSheet.Range["A2:A" + (cnt + 1)]; //Copie noms vers épreuves écrites
                        var to2 = destworkSheet2.Range["A2"];

                        from.Copy(to);
                        from1.Copy(to1);
                        from2.Copy(to2);

                        var cells1 = destworkSheet.Range["B2:B" + (cnt + 1)]; //Copie classe vers récapitulatif
                        cells1.Value = classe;

                        var cells = destworkSheet.Range[
                            "A" + (cnt + 2) + ":A500"]; //Nettoyage bas tableau récapitulatif

                        var del = cells.EntireRow;

                        del.Delete();

                        destworkBook.SaveAs(LblCheminDossierXlsx.Text + @"DNB\" + nommage + classe + "_" +
                                            DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx");
                        srcworkBook.Close();
                        destworkBook.Close();
                        GC.Collect();
                    }
            }
        }

        private void CacherFichiersXlsxDocx()
        {
            var tousLesFichiers = Directory.GetFiles(LblCheminDossierXlsx.Text, "*.*", SearchOption.AllDirectories);
            foreach (var fichier1 in tousLesFichiers)
                if (fichier1.Contains("docx") || fichier1.Contains("xlsx"))
                    File.SetAttributes(fichier1, FileAttributes.Hidden);
        }

        private void ConvertirXlsxEnPdf(string période)
        {
            var appExcel = new Microsoft.Office.Interop.Excel.Application();
            var files = Directory.GetFiles(LblCheminDossierXlsx.Text + période + @"\", "*.xlsx",
                SearchOption.AllDirectories);
            foreach (var line in File.ReadLines(CheminElyco + @"\ELyco\Config\ELyco_classes_annee.txt"))
                foreach (var file in files)
                    if (file.Contains("competence") && file.Contains(line))
                    {
                        var path = Path.GetFullPath(file);
                        var path1 = Path.GetDirectoryName(file);
                        var nomFichier = Path.GetFileNameWithoutExtension(file);
                        var excelDocument = appExcel.Workbooks.Open(path);
                        excelDocument.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF,
                            path1 + @"\" + nomFichier + @".pdf");
                        appExcel.Workbooks.Close();
                        appExcel.Quit();
                        GC.Collect();
                    }
        }

        private int CompterFichiersPrésents(ListBox listbox)
        {
            var countXlsx = 0;
            foreach (var item in listbox.Items)
                if (item.ToString().Contains("competence")) countXlsx++;
            return countXlsx;
        }

        private int CompterFichiersDnb(ListBox listbox)
        {
            var countDocx = 0;
            foreach (var item in listbox.Items)
                if (item.ToString().Contains("DNB1") || item.ToString().Contains("DNB2")) countDocx++;
            return countDocx;
        }

        private void RemplirListeCsvATraiter()
        {
            ListeFichiersPrésents(LblCheminDossierCsv.Text, "", ListBoxCsvATraiter);
        }

        private void RemplirListeCsvPrésents()
        {
            foreach (RadioButton période in PanelTrimestre.Controls)
            {
                ListBoxCsvPrésents.Items.Add(période.Text);
                ListBoxCsvPrésents.Items.Add("-----------------------------------");
                ListeFichiersPrésents(LblCheminDossierCsv.Text, période.Text + @"\", ListBoxCsvPrésents);
                ListBoxCsvPrésents.Items.Add("");
            }
        }

        private void RemplirListeXlsxPrésents()
        {
            foreach (RadioButton période in PanelTrimestre.Controls)
            {
                ListBoxXlsxPrésents.Items.Add(période.Text);
                ListBoxXlsxPrésents.Items.Add("-----------------------------------");
                ListeFichiersPrésents(LblCheminDossierXlsx.Text, période.Text + @"\", ListBoxXlsxPrésents);
                ListBoxXlsxPrésents.Items.Add("");
            }
            ListBoxXlsxPrésents.Items.Add("DNB");
            ListBoxXlsxPrésents.Items.Add("-----------------------------------");
            ListeFichiersPrésents(LblCheminDossierXlsx.Text, "DNB" + @"\", ListBoxXlsxPrésents);
        }

        private void RafraichirListbox()
        {
            EffacerListbox(ListBoxCsvATraiter);
            EffacerListbox(ListBoxCsvPrésents);
            EffacerListbox(ListBoxXlsxPrésents);
            VérifierCheminsDossiers();
            RemplirListeCsvPrésents();
            RemplirListeXlsxPrésents();
            RemplirListeCsvATraiter();
            LblFichiersCsvATraiter.Text = ListBoxCsvATraiter.Items.Count + @" classes à traiter";
            LblFichiersCsvPrésents.Text = CompterFichiersPrésents(ListBoxCsvPrésents) + @" fichiers CSV";
            LblFichiersXlsxPrésents.Text = CompterFichiersPrésents(ListBoxXlsxPrésents) + @" fichiers XLSX et " +
                                           CompterFichiersDnb(ListBoxXlsxPrésents) + @" fichiers DNB";

            SélectionPériode(new object(), new EventArgs());
        }

        private string DétectionPériode()
        {
            string périodeSelect = null;
            foreach (RadioButton période in PanelTrimestre.Controls)
                if (période.Checked)
                    périodeSelect = période.Text;
            return périodeSelect;
        }

        private void SupprimerObjets(object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
            }
            catch
            {
                // ignored
            }
            finally
            {
                GC.Collect();
            }
        }

        private void ExécuterMacro(string macro)
        {
            //~~> Define your Excel Objects
            var xlApp = new Microsoft.Office.Interop.Excel.Application();

            var sPath = Path.GetTempFileName();
            File.WriteAllBytes(sPath, Resources.Compétences);

            //~~> Start Excel and open the workbook.
            var xlWorkBook = xlApp.Workbooks.Open(sPath);

            //~~> Run the macros by supplying the necessary arguments
            xlApp.Run(macro);

            //~~> Clean-up: Close the workbook
            xlWorkBook.Close(false);

            //~~> Quit the Excel Application
            xlApp.Quit();

            //~~> Clean Up
            SupprimerObjets(xlApp);
            SupprimerObjets(xlWorkBook);
        }

        private void ResetComboNiveau()
        {
            foreach (ComboBox combo in PanelClasses.Controls)
            {
                combo.Items.Clear();
                for (var i = 0; i < 13; i++)
                    combo.Items.Add(i);
                combo.SelectedIndex = 0;
            }
        }
    }
}