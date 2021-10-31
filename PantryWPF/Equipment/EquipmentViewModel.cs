using Pantry.Core.Models;
using Pantry.ServiceGateways.Equipment;
using Pantry.WPF.Shared;
using Serilog.Core;
using Stylet;

namespace Pantry.WPF.Equipment
{
    public class EquipmentViewModel : Screen
    {
        private readonly EquipmentServiceGateway _equipmentSg;
        private readonly Logger _logger;

        public BindableCollection<Core.Models.Equipment> Equipments { get; set; }
        public BindableCollection<EquipmentType> EquipmentTypes { get; set; }
        public DelegateCommand AddEquipmentDelegateCommand { get; }

        private EquipmentType _selectedEquipmentType;
        public EquipmentType SelectedEquipmentType
        {
            get => _selectedEquipmentType;
            set => SetAndNotify(ref _selectedEquipmentType, value, nameof(SelectedEquipmentType));
        }

        private string _newEquipmentName;
        public string NewEquipmentName
        {
            get => _newEquipmentName;
            set => SetAndNotify(ref _newEquipmentName, value, nameof(NewEquipmentName));
        }

        public EquipmentViewModel(EquipmentServiceGateway equipmentSg, Logger logger)
        {
            _equipmentSg = equipmentSg;
            _logger = logger;
            AddEquipmentDelegateCommand = new(AddEquipment);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            LoadData();
        }

        private void LoadData()
        {
            _logger.Debug("Loading EquipmentViewModel.");
            LoadEquipments();
            LoadEquipmentTypes();
            _logger.Debug("Loaded EquipmentViewModel.");
        }

        private void LoadEquipmentTypes()
        {
            var equipmentTypes = _equipmentSg.GetEquipmentTypes();
            if (EquipmentTypes is null)
            {
                EquipmentTypes = new(equipmentTypes);
            }
            else
            {
                EquipmentTypes.Clear();
                EquipmentTypes.AddRange(equipmentTypes);
            }
        }

        private void LoadEquipments()
        {
            var equipments = _equipmentSg.GetEquipments();
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
            if (string.IsNullOrWhiteSpace(NewEquipmentName) || SelectedEquipmentType is null) { return; }
            _equipmentSg.AddEquipment(NewEquipmentName, SelectedEquipmentType.EquipmentTypeId);
            NewEquipmentName = "";
            SelectedEquipmentType = null;
            LoadEquipments();
        }

    }
}
