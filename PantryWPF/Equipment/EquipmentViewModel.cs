using System.Threading.Tasks;
using System.Windows;
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

        protected override async void OnActivate()
        {
            base.OnActivate();
            await LoadData();
        }

        private async Task LoadData()
        {
            _logger.Debug("Loading EquipmentViewModel.");
            await LoadEquipments();
            await LoadEquipmentTypes();
            _logger.Debug("Loaded EquipmentViewModel.");
        }

        private async Task LoadEquipmentTypes()
        {
            var equipmentTypes = await _equipmentSg.GetEquipmentTypes();
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

        private async Task LoadEquipments()
        {
            var equipments = await _equipmentSg.GetEquipments();
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

        private async Task AddEquipment()
        {
            if (string.IsNullOrWhiteSpace(NewEquipmentName) || SelectedEquipmentType is null) { return; }
            var addSuccess = await _equipmentSg.AddEquipment(NewEquipmentName, SelectedEquipmentType.EquipmentTypeId);
            if (!addSuccess)
            {
                MessageBox.Show("Locations must first be populated");
                return;
            }
            NewEquipmentName = "";
            SelectedEquipmentType = null;
            LoadEquipments();
        }

    }
}
