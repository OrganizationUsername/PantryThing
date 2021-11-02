using Pantry.ServiceGateways.Equipment;
using Stylet;
using System.Threading.Tasks;

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

        public async Task Load(int equipmentTypeId)
        {
            var equipments = await _equipmentServiceGateway.GetEquipments(equipmentTypeId);
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