using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pantry.ServiceGateways;
using Pantry.WPF.Shared;
using Stylet;

namespace Pantry.WPF.Equipment
{
    public class EquipmentViewModel : Screen
    {
        private readonly ItemService _itemService;

        public BindableCollection<Core.Models.Equipment> Equipments { get; set; }
        public DelegateCommand AddEquipmentDelegateCommand { get; }

        private string _newEquipmentName;
        public string NewEquipmentName
        { 
            get => _newEquipmentName;
            set => SetAndNotify(ref _newEquipmentName, value, nameof(NewEquipmentName));
        }

        public EquipmentViewModel(ItemService itemService)
        {
            _itemService = itemService;
            AddEquipmentDelegateCommand = new(AddEquipment);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
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
                Equipments.AddRange(equipments);
            }
        }

        private void AddEquipment()
        {
            if (string.IsNullOrWhiteSpace(NewEquipmentName)) { return; } //ToDo: Now I know that Equipment needs to be connected to EquipmentInstance or I need to have EquipmentTypes.
            _itemService.AddEquipment(NewEquipmentName);
            NewEquipmentName = "";
            LoadData();
        }
    }
}
