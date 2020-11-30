using Bakery.Core.Contracts;
using Bakery.Core.DTOs;
using Bakery.Core.Entities;
using Bakery.Persistence;
using Bakery.Wpf.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bakery.Wpf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private List<ProductDto> _allProducts;
        private ObservableCollection<ProductDto> _products;
        private string _priceFrom;
        private string _priceTo;
        private ProductDto _selectedProduct;

        public string PriceFrom
        {
            get => _priceFrom; set
            {
                _priceFrom = value;
                OnPropertyChanged(nameof(PriceFrom));
            }
        }
        public string PriceTo
        {
            get => _priceTo; set
            {
                _priceTo = value;
                OnPropertyChanged(nameof(PriceTo));
            }
        }
        public ProductDto SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }
        public ObservableCollection<ProductDto> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }
        public double AvgPrice => Products.Average(p => p.Price);

        public MainWindowViewModel(IWindowController controller) : base(controller)
        {
        }

        public static async Task<MainWindowViewModel> Create(IWindowController controller)
        {
            var model = new MainWindowViewModel(controller);
            await model.LoadProducts();
            return model;
        }

        /// <summary>
        /// Produkte laden. Produkte können nach Preis gefiltert werden. 
        /// Preis liegt zwischen from und to
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private async Task LoadProducts()
        {
            IUnitOfWork uow = new UnitOfWork();
            var products = await uow.Products
                .GetAllAsync();

            _allProducts = new List<ProductDto>(products
                .Select(p => new ProductDto(p)));

            Products = new ObservableCollection<ProductDto>(products
                .Select(p => new ProductDto(p)));

            SelectedProduct = Products.FirstOrDefault();
        }

        private void RefreshDataGrid()
        {
            try
            {
                if (PriceFrom != null && PriceTo != null)
                {
                    double priceFrom = Convert.ToDouble(PriceFrom);
                    double priceTo = Convert.ToDouble(PriceTo);
                    Products = new ObservableCollection<ProductDto>(_allProducts
                        .Where(p => p.Price >= priceFrom && p.Price <= priceTo));
                }
                else if (PriceFrom != null)
                {
                    double priceFrom = Convert.ToDouble(PriceFrom);
                    Products = new ObservableCollection<ProductDto>(_allProducts
                        .Where(p => p.Price >= priceFrom));
                }
                else if (PriceTo != null)
                {
                    double priceTo = Convert.ToDouble(PriceTo);
                    Products = new ObservableCollection<ProductDto>(_allProducts
                        .Where(p => p.Price <= priceTo));
                }
            }
            catch (Exception ex)
            {
                Products = new ObservableCollection<ProductDto>(_allProducts);
            }
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }

        private ICommand _cmdSearchCommand;
        public ICommand CmdSearchCommand
        {
            get
            {
                if (_cmdSearchCommand == null)
                {
                    _cmdSearchCommand = new RelayCommand(
                        execute: _ =>
                        {
                            RefreshDataGrid();
                        },
                        canExecute: _ => PriceFrom != null || PriceTo != null
                        );
                }

                return _cmdSearchCommand;
            }
            set => _cmdSearchCommand = value;
        }

        private ICommand _cmdCreateCommand;
        public ICommand CmdCreateCommand
        {
            get
            {
                if (_cmdCreateCommand == null)
                {
                    _cmdCreateCommand = new RelayCommand(
                        execute: _ =>
                        {
                            Controller.ShowWindow(new EditAndCreateProductViewModel(Controller, null), true);
                            _ = LoadProducts();
                        },
                        canExecute: _ => true
                        );
                }

                return _cmdCreateCommand;
            }
            set => _cmdCreateCommand = value;
        }

        private ICommand _cmdEditCommand;
        public ICommand CmdEditCommand
        {
            get
            {
                if (_cmdEditCommand == null)
                {
                    _cmdEditCommand = new RelayCommand(
                        execute: _ =>
                        {
                            Controller.ShowWindow(new EditAndCreateProductViewModel(Controller, SelectedProduct), true);
                            _ = LoadProducts();
                        },
                        canExecute: _ => true
                        );
                }

                return _cmdEditCommand;
            }
            set => _cmdEditCommand = value;
        }
    }
}
