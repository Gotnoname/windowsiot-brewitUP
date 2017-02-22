using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrewLib.Objects;
using BrewLib.Profile;
using BrewLib.DropSlot;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace BrewLib.Interfaces.Implementations
{
    public class IngredientStep : BaseStep
    {
        public IngredientStep()
        {
            Type = StepType.Ingredient;
        }

        public override async Task RunTaskAsync(CancellationToken cancelToken)
        {
            using (var buzzer = new BuzzerController())
            {
                buzzer.ActivateBuzzer(cancelToken);
            }

            StepDropSlotController.Instance.DropStep(this);

            UIMessager.Instance.ShowMessage($"{Type}", 
                $"{Title} should be added now. The amount to add: {Amount}.",
                UIMessageButtons.OK,
                UIMessageType.Information);

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                ProgressPercent = 100;
                Finished = true;
            });
        }
    }
}
