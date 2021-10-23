using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pantry.ServiceGateways;
using Pantry.WPF.Shared;

namespace Pantry.WPF.Equipment
{
    public class EquipmentViewModel : VmBase
    {
        private ItemService _itemService;

        public ObservableCollection<Core.Models.Equipment> Equipments { get; set; }
        public DelegateCommand AddEquipmentDelegateCommand { get; set; }
        public string NewEquipmentName { get; set; }
        public EquipmentViewModel()
        {
            _itemService = new();
            AddEquipmentDelegateCommand = new(AddEquipment);
            LoadData();

        }

        private void LoadData()
        {
            var equipments = _itemService.GetEquipments();
            if (Equipments is null)
            {
                Equipments = new(equipments);
            }
            else
            {
                Equipments.Clear();
                foreach (var equipment in equipments)
                {
                    Equipments.Add(equipment);
                }
            }
        }

        private void AddEquipment()
        {
            if (string.IsNullOrWhiteSpace(NewEquipmentName)) { return; } //ToDo: Now I know that Equipment needs to be connected to EquipmentInstance or I need to have EquipmentTypes.
            _itemService.AddEquipment(NewEquipmentName);
            NewEquipmentName = "";
            OnPropertyChanged(nameof(NewEquipmentName));
            LoadData();
        }


    }
}
