using System;
using System.Collections.Generic;
using System.Linq;

namespace LuccaDevises
{
    class DeviseCalculator : IDisposable
    {
        //Liste de solutions à créer 
        public List<DeviseSolution> solutions = new List<DeviseSolution>();

        public DeviseSolution TrouverDeviseSolution(DeviseSolution deviseSolution, List<TauxChange> listDiffDeviseInitiale, DeviseObject deviseObject)
        {
            //Trouver la liste restante à traiter pour chercher les taux de changes .
            var listDiffDeviseInitialeFiltered = listDiffDeviseInitiale.Where(i => i.DeviseArrivee == deviseSolution.DeviseArrivee || i.DeviseDepart == deviseSolution.DeviseArrivee).ToList();

            //Créer une liste pour enlever les éléments qui sont déjà ajoutés à DeviseSolution

            List<TauxChange> listRemove = new List<TauxChange>();
            foreach (var item in listDiffDeviseInitialeFiltered)
            {
                if (deviseSolution.ListeTauxChange.Where(ds => ds.DeviseArrivee == item.DeviseArrivee && ds.DeviseDepart == item.DeviseDepart).FirstOrDefault() != null)
                {
                    listRemove.Add(item);
                }
            }
            foreach (var itemRemove in listRemove)
            {
                //Suppression d'item existant 
                listDiffDeviseInitialeFiltered.Remove(itemRemove);
            }
            if (listDiffDeviseInitialeFiltered.Count() == 0)
            {
                //Si Count = 0 donc on doit arrêter le traitement pour DeviseSolution. 
                deviseSolution.Termniee = true;
            }
            else
            {
                //Ajouter la première ligne
                deviseSolution.ListeTauxChange.Add(listDiffDeviseInitialeFiltered.FirstOrDefault());
                if (listDiffDeviseInitialeFiltered.FirstOrDefault().DeviseDepart == deviseSolution.DeviseArrivee)
                {
                    //Mise à jour de DeviseSolution.DeviseArrivee
                    //Vérifier si on a trouvé la devise cible ou pas 
                    deviseSolution.DeviseArrivee = listDiffDeviseInitialeFiltered.FirstOrDefault().DeviseArrivee;
                    if (deviseSolution.DeviseArrivee == deviseObject.DeviseCible)
                    { deviseSolution.Termniee = true; }
                }
                else
                {
                    //Mise à jour de DeviseSolution.DeviseArrivee
                    //Vérifier si on a trouvé la devise cible ou pas 

                    deviseSolution.DeviseArrivee = listDiffDeviseInitialeFiltered.FirstOrDefault().DeviseDepart;
                    if (deviseSolution.DeviseArrivee == deviseObject.DeviseCible)
                    { deviseSolution.Termniee = true; }
                }
                while (deviseSolution.DeviseArrivee != deviseObject.DeviseCible && !deviseSolution.Termniee)
                {
                    //Tant qu'on trouve pas la devise cible ou bien on termine pas le traitement de la liste on continue la recherche 
                    deviseSolution = TrouverDeviseSolution(deviseSolution, listDiffDeviseInitiale, deviseObject);
                }
                //Skip de la première ligne car on doit créer des nouvelles solution si on trouve plusieurs lignes 
                foreach (var item in listDiffDeviseInitialeFiltered.Skip(1))
                {
                    //Créer une nouvelle solution qui sera égale à la première solution 
                    DeviseSolution newDeviseSolution = deviseSolution;
                    solutions.Add(newDeviseSolution);
                    newDeviseSolution = TrouverDeviseSolution(newDeviseSolution, listDiffDeviseInitiale, deviseObject);
                    while (deviseSolution.DeviseArrivee != deviseObject.DeviseCible && !deviseSolution.Termniee)
                    {
                        deviseSolution = TrouverDeviseSolution(deviseSolution, listDiffDeviseInitiale, deviseObject);
                    }
                }
            }
            return deviseSolution;
        }
        public decimal CalculateDevise(DeviseObject deviseObject)
        {

            decimal resultat = 0;
            //Commencer par trouver la liste des taux de change qui contiennent la devise initiale 
            var listDeviseInitiale = deviseObject.ListeTauxChange.Where(t => t.DeviseDepart == deviseObject.DeviseInitiale || t.DeviseArrivee == deviseObject.DeviseInitiale);
            //Filtrer la liste totale et enlever les lignes de la liste des taux de change qui contiennent la devise initiale  
            var listDiffDeviseInitiale = deviseObject.ListeTauxChange.Where(t => t.DeviseDepart != deviseObject.DeviseInitiale && t.DeviseArrivee != deviseObject.DeviseInitiale).ToList();
            //Parcourir la liste listDeviseInitiale et créer une solution pour chaque ligne 
            foreach (var itemDeviseInitiale in listDeviseInitiale)
            {
                //Créer une nouvelle solution devise 
                DeviseSolution deviseSolution = new DeviseSolution();
                deviseSolution.ListeTauxChange.Add(
                    new TauxChange()
                    {
                        DeviseDepart = itemDeviseInitiale.DeviseDepart,
                        DeviseArrivee = itemDeviseInitiale.DeviseArrivee,
                        Taux = itemDeviseInitiale.Taux
                    });
                solutions.Add(deviseSolution);
                //Définir devise à chercher 
                string deviseInitialeAttachee = string.Empty;
                if (itemDeviseInitiale.DeviseDepart == deviseObject.DeviseInitiale)
                {
                    deviseInitialeAttachee = itemDeviseInitiale.DeviseArrivee;
                }
                else
                 if (itemDeviseInitiale.DeviseDepart == deviseObject.DeviseCible)
                {
                    deviseInitialeAttachee = itemDeviseInitiale.DeviseDepart;
                }
                deviseSolution.DeviseArrivee = deviseInitialeAttachee;
                //Appeler la fonction récursive TrouverDeviseSolution pour trouver tous les solutions possibles 
                TrouverDeviseSolution(deviseSolution, listDiffDeviseInitiale, deviseObject);
            }
            //Récupérer la solution la plus rapide 
            DeviseSolution deviseSolutionOptimale = solutions?.Where(s => s.DeviseArrivee == deviseObject.DeviseCible).OrderBy(s => s.ListeTauxChange.Count()).FirstOrDefault();
            if (deviseSolutionOptimale != null)
            {
                //Calculer le montant
                //Sevise Départ = DeviseInitiale
                String DD = deviseObject.DeviseInitiale;
                foreach (var item in deviseSolutionOptimale.ListeTauxChange)
                {
                    //Parcourir la liste de taux de change et calculer le montant par rapport au taux (1/Taux si on calcule à partir de Devise Arrivee)
                    if (DD == item.DeviseArrivee)
                    {
                        resultat = deviseObject.Montant * Math.Round(1 / item.Taux, 4);
                        DD = item.DeviseDepart;
                    }
                    else
                    {
                        resultat = deviseObject.Montant * item.Taux;
                        DD = item.DeviseArrivee;
                    }
                    deviseObject.Montant = resultat;

                }
            }
            //Round resultat
            resultat = decimal.Round(resultat);
            return resultat;

        }

        void ReleaseUnmanagedResources()
        {
            solutions = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DeviseCalculator()
        {
            Dispose(false);
        }
    }
}
