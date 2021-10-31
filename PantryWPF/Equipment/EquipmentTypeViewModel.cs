using Pantry.Core.Models;
using Pantry.ServiceGateways;
using Pantry.ServiceGateways.Equipment;
using Stylet;

namespace Pantry.WPF.Equipment
{
    public class EquipmentTypeViewModel : Screen
    {
        private readonly EquipmentServiceGateway _equipmentServiceGateway;
        public string Test { get; set; } = "test";
        public BindableCollection<EquipmentType> EquipmentTypes { get; set; }

        public EquipmentTypeViewModel(EquipmentServiceGateway equipmentServiceGateway)
        {
            _equipmentServiceGateway = equipmentServiceGateway;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            LoadData();
        }

        private void LoadData()
        {
            if (EquipmentTypes is null)
            {
                EquipmentTypes = new(_equipmentServiceGateway.GetEquipmentTypes());
            }
            else
            {
                EquipmentTypes.Clear();
                EquipmentTypes.AddRange(_equipmentServiceGateway.GetEquipmentTypes());
            }

        }
    }
}
