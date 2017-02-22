using BrewLib.Interfaces;
using BrewLib.Interfaces.Implementations;
using BrewLib.Profile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewLib
{
    //The beer brew profile containing all
    //required information about how to brew this 
    //specific beer.
    public class BrewProfile : INotifyPropertyChanged
    {
        #region Privates
        private int _boilTime;
        #endregion

        public Guid Id { get; set; }
        public string Name { get; private set; }
        public int BoilTime { get; set; }
        public List<IStep> Steps { get; set; }
        public bool DelayedStart { get; set; }             

        public int MashTime
        {
            get
            {
                return Steps.Where(i => i.Type == Objects.StepType.Mash).Sum(s => s.LengthMinutes);
            }
        }

        public BrewProfile()
        {
            Steps = new List<IStep>();
        }

        public BrewProfile(string name, int boilTime, List<IStep> steps)
        {
            Name = name;
            BoilTime = boilTime;
            Steps = steps;
        }

        public static BrewProfile GetTestProfile()
        {
#if DEBUG
            var steps = new List<IStep>();
            steps.Add(new MashingStep()
            {
                Temperature = 30,
                LengthMinutes = 2,                
                Type = Objects.StepType.Mash,
            });
            steps.Add(new MashingStep()
            {
                Temperature = 35,
                LengthMinutes = 2,
                Type = Objects.StepType.Mash,
            });

            steps.Add(new BoilStep()
            {
                Temperature = 98,
                LengthMinutes = 5,
                Type = Objects.StepType.Boil,
                SubSteps = new System.Collections.ObjectModel.ObservableCollection<IStep>
                {
                    new IngredientStep() {Title = "Chinook", Amount = 10, LengthMinutes = 90 - 60 },
                    new IngredientStep() {Title = "Ahtanum", Amount = 35, LengthMinutes = 90 - 15 },
                    new IngredientStep() {Title = "Cascade", Amount = 35, LengthMinutes = 90 - 2 },
                }
            });
            return new BrewProfile("American Brown Ale", 90, steps);
#endif
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
