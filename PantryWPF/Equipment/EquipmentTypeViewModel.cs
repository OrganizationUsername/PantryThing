using System;
using System.Windows;
using Pantry.Core.Models;
using Pantry.ServiceGateways.Equipment;
using Pantry.WPF.Shared;
using Stylet;

namespace Pantry.WPF.Equipment
{
    public class EquipmentTypeViewModel : Screen
    {
        private readonly EquipmentServiceGateway _equipmentServiceGateway;
        private readonly Func<EquipmentTypeDetailViewModel> _equipmentTypeDetailFactory;
        private EquipmentType _selectedEquipmentType;
        private EquipmentTypeDetailViewModel _selectedEquipmentTypeDetailViewModel;
        private string _newEquipmentType;
        public DelegateCommand AddEquipmentTypeDelegateCommand { get; set; }
        public BindableCollection<EquipmentType> EquipmentTypes { get; set; }

        public string NewEquipmentType
        {
            get => _newEquipmentType;
            set => SetAndNotify(ref _newEquipmentType, value, nameof(NewEquipmentType));
        }

        public EquipmentTypeDetailViewModel SelectedEquipmentTypeDetailViewModel
        {
            get => _selectedEquipmentTypeDetailViewModel;
            set => SetAndNotify(ref _selectedEquipmentTypeDetailViewModel, value, nameof(SelectedEquipmentTypeDetailViewModel));
        }

        public EquipmentType SelectedEquipmentType
        {
            get => _selectedEquipmentType;
            set
            {
                if (SetAndNotify(ref _selectedEquipmentType, value, nameof(SelectedEquipmentType)))
                {
                    if (_selectedEquipmentType is null) return;
                    SelectedEquipmentTypeDetailViewModel = _equipmentTypeDetailFactory();
                    SelectedEquipmentTypeDetailViewModel?.Load(_selectedEquipmentType.EquipmentTypeId);

                }
            }
        }

        public EquipmentTypeViewModel(EquipmentServiceGateway equipmentServiceGateway, Func<EquipmentTypeDetailViewModel> equipmentTypeDetailFactory)
        {
            _equipmentServiceGateway = equipmentServiceGateway;
            _equipmentTypeDetailFactory = equipmentTypeDetailFactory;
            AddEquipmentTypeDelegateCommand = new(AddEquipmentType);

        }

        public void AddEquipmentType()
        {
            if (_equipmentServiceGateway.AddEquipmentType(NewEquipmentType))
            {
                NewEquipmentType = "";
                LoadData();
            }
            else
            {
                MessageBox.Show("Equipment Type with same name already exists or new Equipment Type was not meaningful.");
            }
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
