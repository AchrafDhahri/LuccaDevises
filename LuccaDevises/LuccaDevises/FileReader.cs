using System;
using System.IO;
using System.Linq;

namespace LuccaDevises
{

    static class FileReader
    {
        public static DeviseObject ReadFile(string path)
        {
            //Create New DeviseObject
            DeviseObject deviseObject = new DeviseObject();
            try
            {
                //Read Lines from file 
                string[] lines = File.ReadAllLines(path);
                if (lines != null && lines.Count() > 0)
                {
                    //Line 1
                    string[] line1 = lines[0].Split(';');
                    if (line1.Count() == 3)
                    {
                        deviseObject.DeviseInitiale = line1[0];
                        deviseObject.Montant = decimal.Parse(line1[1].Replace('.', ','));
                        deviseObject.DeviseCible = line1[2];
                        if (deviseObject.Montant < 0)
                        {
                            Console.WriteLine("Veuillez vérifier le fichier d'entrée , le montant est négatif !");
                            return null;

                        }
                        if (deviseObject.DeviseInitiale.Length != 3)
                        {
                            Console.WriteLine("Veuillez vérifier le fichier d'entrée , D1 incorrect !");

                            return null;

                        }
                        if (deviseObject.DeviseCible.Length != 3)
                        {
                            Console.WriteLine("Veuillez vérifier le fichier d'entrée , D2 incorrect !");
                            return null;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Veuillez vérifier le fichier d'entrée , première ligne incorrecte !");
                        return null;
                    }

                    if (deviseObject.DeviseInitiale == deviseObject.DeviseCible)
                    {
                        //Cas exceptionnel si on mets la même devise cible et initiale 
                        Console.WriteLine("Veuillez vérifier le fichier d'entrée , devise initiale et cible incorrectes !");
                        return null;

                    }
                    //Line 2
                    string line2 = lines[1];
                    if (!string.IsNullOrEmpty(line2))
                    {
                        deviseObject.NbreTauxChange = int.Parse(line2);
                        if (deviseObject.NbreTauxChange <= 0 || (deviseObject.NbreTauxChange != lines.Count() - 2))
                        {
                            Console.WriteLine("Veuillez vérifier le fichier d'entrée , nombre de taux de changes incorrect !");
                            return null;
                        }

                    }
                    else
                    {
                        Console.WriteLine("Veuillez vérifier le fichier d'entrée , nombre de taux de changes null !");
                        return null;
                    }
                    //Parcourir le reste de lignes pour récupérer les taux de change 
                    for (int i = 2; i < lines.Count(); i++)
                    {
                        string lineTauxChange = lines[i];
                        if (!string.IsNullOrEmpty(lineTauxChange))
                        {
                            string[] lineTauxChangeValues = lineTauxChange.Split(';');
                            if (lineTauxChangeValues != null && lineTauxChangeValues.Count() == 3)
                            {
                                deviseObject.ListeTauxChange.Add(
                                    new TauxChange()
                                    {
                                        DeviseDepart = lineTauxChangeValues[0],
                                        DeviseArrivee = lineTauxChangeValues[1],
                                        Taux = decimal.Parse(lineTauxChangeValues[2].Replace('.', ','))
                                    });
                            }
                            else
                            {
                                Console.WriteLine("Veuillez vérifier le fichier d'entrée , ligne de taux de changes incorrecte !");
                                return null;
                            }

                        }
                        else
                        {
                            Console.WriteLine("Veuillez vérifier le fichier d'entrée , ligne de taux de changes null !");
                            return null;
                        }

                    }
                    if (deviseObject.ListeTauxChange == null || deviseObject.ListeTauxChange.Count() != deviseObject.NbreTauxChange)
                    {
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine("Veuillez vérifier le fichier d'entrée ! , fichier vide !");
                    return null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'exécution du programme !");
                Console.WriteLine("Détails de l'erreur : " + "\n" + ex.Message + ex.StackTrace);
                return null;

            }
            return deviseObject;
        }
    }
}
