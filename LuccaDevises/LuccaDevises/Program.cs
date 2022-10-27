using System;
using System.IO;

namespace LuccaDevises
{
    public static class Program
    {
        static void Main(string[] args)
        {
            //Vérification des arguments 
            if (args.Length < 1 || args.Length > 2)
            {
                Console.WriteLine("Veuillez vérifier les paramètres d'éxecution !");
                Console.ReadLine();
                return;
            }
            //Vérification de la commande LuccaDevises
            var commandeLuccaDevises = args[0];
            if (commandeLuccaDevises != Config.LuccaDevises)
            {
                Console.WriteLine("Veuillez vérifier les paramètres d'éxecution, commande incorrecte !");
                Console.ReadLine();
                return;
            }

            var filePath = args[1];
            if (!File.Exists(filePath))
            {
                //Vérifier si le fichier existe 
                Console.WriteLine("Fichier introuvable !");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Fichier : " + filePath);

            //Créer l'objet DeviseObject à partir du fichier 
            using (DeviseObject deviseObject = FileReader.ReadFile(filePath))
            {
                //Utiliser Using pour libérer la mémoire et disposer les objets 
                if (deviseObject != null)
                {
                    Console.WriteLine(string.Empty);
                    Console.WriteLine("Entrée ");
                    Console.WriteLine("Devise Initiale  : " + deviseObject.DeviseInitiale);
                    Console.WriteLine("Devise Cible  : " + deviseObject.DeviseCible);
                    Console.WriteLine("Montant : " + deviseObject.Montant.ToString());
                    Console.WriteLine("Nombre taux de change : " + deviseObject.NbreTauxChange.ToString());
                    Console.WriteLine("Liste taux de change : ");
                    foreach (var item in deviseObject.ListeTauxChange)
                    {
                        Console.WriteLine(item.DeviseDepart + ";" + item.DeviseArrivee + ";" + item.Taux);
                    }

                    //Calculer ...
                    using (DeviseCalculator deviseCalculator = new DeviseCalculator())
                    {
                        decimal res = deviseCalculator.CalculateDevise(deviseObject);
                        Console.WriteLine(string.Empty);
                        Console.WriteLine("Sortie ");
                        Console.WriteLine(res.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Veuillez vérifier le fichier donnèes incorrectes !");
                }

            }

        }

    }
}
