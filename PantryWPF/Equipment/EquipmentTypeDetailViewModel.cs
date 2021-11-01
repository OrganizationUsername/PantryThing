using Pantry.ServiceGateways.Equipment;
using Stylet;

namespace Pantry.WPF.Equipment
{
    public class EquipmentTypeDetailViewModel : Screen
    {
        private readonly EquipmentServiceGateway _equipmentServiceGateway;
        public BindableCollection<Core.Models.Equipment> Equipments { get; set; }

        public EquipmentTypeDetailViewModel(EquipmentServiceGateway equipmentServiceGateway)
        {
            _equipmentServiceGateway = equipmentServiceGateway;
        }

        public void Load(int equipmentTypeId)
        {
            var equipments = _equipmentServiceGateway.GetEquipments(equipmentTypeId);
            if (Equipments is null)
            {
                Equipments = new(equipments);
            }
            else
            {
                Equipments.Clear();
                equipments.AddRange(equipments);
            }
        }

    }
}