﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using AppsToolkit;

namespace Browser_Memory_Logger
{
    public partial class Form1 : Form
    {

        List<string> detectedApps = new List<string>();                 // La liste des applications détectées
        CSVTools csvManager = new CSVTools(Application.ProductName);    // On créé le gestionnaire de CSV

        public Form1()
        {
            InitializeComponent();

            // On récupère le nom et la version de l'application pour le titre de la fenêtre
            this.Text = String.Format("{0} v{1}", Application.ProductName, Application.ProductVersion);

            // On supprime les fichiers CSV précédents
            csvManager.Clean();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // On met à jour la ListBox
            updateResult(lbResult);

            // S'il n'y a pas au moins un processus détecté, on quitte l'application
            string msg_noProcessDetected = "Aucun navigateur n'a été reconnu, l'application va se fermer";
            if (lbResult.Items.Count < 1)
            {
                MessageBox.Show(msg_noProcessDetected, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Application.Exit();
            }

            // On démarre le timer
            Timer tUpdate = new Timer();
            tUpdate.Interval = 1000;
            tUpdate.Tick += tUpdate_Tick;
            tUpdate.Start();
        }

        // La fonction qui met à jour la ListBox
        private void updateResult(ListBox lb)
        {
            // On enregistre l'index actuel pour utilisation après l'update
            int oldIndex = lb.SelectedIndex;

            // On récupère la liste des process
            Process[] processes = Process.GetProcesses();

            // On définit une liste des applications pouvant être reconnues
            string[] supportedProcessNames = { "firefox", "chrome", "iexplore", "opera", "microsoftedge", "vivaldi" };
            List<string> supportedApps = new List<string>(supportedProcessNames);
            supportedApps.Sort();

            // On nettoie la TextBox de sortie
            lbResult.Items.Clear();

            // Pour chaque processus de la liste
            foreach (Process p in processes)
            {
                // On améliore l'affichage du nom du processus
                string beautifiedName = p.ProcessName.ToString().Substring(0, 1).ToUpper() + p.ProcessName.ToString().Substring(1).ToLower();

                // Si le processus est lancé et qu'il n'est pas déjà dans la liste, on le rajoute
                if (supportedApps.Contains(p.ProcessName.ToString().ToLower()) & !detectedApps.Contains(beautifiedName))
                {
                    detectedApps.Add(beautifiedName);
                }
            }

            // Pour chaque application détectée
            foreach (string a in detectedApps)
            {
                // On récupère les infos du processus et on rajoute une ligne dans la ListBox
                long[] pMemInfos = ProcessTools.getUsedMem(a);

                // On gère le cas spécifique de Microsoft Edge qui a deux noms de processus différents
                if (a == "Microsoftedge")
                {
                    long[] pMemInfosCP = ProcessTools.getUsedMem("Microsoftedgecp");
                    pMemInfos[0] += pMemInfosCP[0];
                    pMemInfos[1] += pMemInfosCP[1];
                }

                lbResult.Items.Add(string.Format("{0} : {1}", a, ProcessTools.getMemUsedString(pMemInfos)));

                // Si l'on a demandé un log dans le fichier CSV, on rajoute les valeurs récupérées
                if (chkLog.Checked) csvManager.Append(pMemInfos[0], a);
            }

            // Une fois l'update terminée, on récupère l'ancien index. S'il est de -1, on le met à 0
            lb.SelectedIndex = oldIndex == -1 ? 0 : oldIndex;
        }

        // Lorsque l'on demande à ouvrir le fichier CSV
        private void btnCSVOpen_Click(object sender, EventArgs e)
        {
            // On récupère le nom du fichier et on l'ouvre
            string fName = selectedProcessCSVFileName();
            if (File.Exists(fName)) Process.Start(fName);
        }

        // La fonction qui récupère le nom du fichier CSV en fonction de la ligne sélectionnée dans la ListBox
        private string selectedProcessCSVFileName()
        {
            string process = detectedApps[lbResult.SelectedIndex];
            return csvManager.GetFilename(process);
        }

        // La fonction qui détermine si le bouton d'ouverure du CSV doit être actif ou non
        private void checkOpenCSVButtonState()
        {
            btnCsvOpen.Enabled = File.Exists(selectedProcessCSVFileName()) & !chkLog.Checked ? true : false;
        }

        // Lorsque l'on clique sur le lien
        private void lnk_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.nextinpact.com");
        }

        // Lorsque le timer s'active
        private void tUpdate_Tick(object sender, EventArgs e)
        {
            // On vérifie l'état du bouton d'ouverture du CSV et on met à jour la ListBox
            checkOpenCSVButtonState();
            updateResult(lbResult);
        }

        // Lorsque l'on change de sélection dans la ListBox
        private void lbResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            // On vérifie l'état du bouton d'ouverture du CSV
            checkOpenCSVButtonState();
        }

        // Lorsque l'on demande un nettoyage des fichiers CSV
        private void btnCsvClean_Click(object sender, EventArgs e)
        {
            // On supprime les fichiers CSV, puis on vérifie l'état du bouton d'ouverture du fichier CSV
            csvManager.Clean();
            checkOpenCSVButtonState();
        }
    }
}
