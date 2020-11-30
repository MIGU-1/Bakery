using Bakery.Core.Contracts;
using Bakery.Core.DTOs;
using Bakery.Core.Entities;
using Bakery.Persistence;
using Bakery.Wpf.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows.Input;

namespace Bakery.Wpf.ViewModels
{
    class EditAndCreateProductViewModel : BaseViewModel
    {
        private bool isCreate = false;
        private Product _product;
        private string _productNr;
        private string _productName;
        private string _price;

        public Product Product
        {
            get => _product;
            set
            {
                _product = value;
                OnPropertyChanged(nameof(Product));
            }
        }
        public string ProductNr
        {
            get => _productNr;
            set
            {
                _productNr = value;
                OnPropertyChanged(nameof(ProductNr));
                ValidateViewModelProperties();
            }
        }

        [MaxLength(20)]
        public string Name
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged(nameof(Name));
                ValidateViewModelProperties();
            }
        }
        public string Price
        {
            get => _price;
            set
            {
                _price = value;
                OnErrorsChanged(nameof(Price));
                ValidateViewModelProperties();
            }
        }

        public EditAndCreateProductViewModel(IWindowController controller, ProductDto product) : base(controller)
        {
            if (product != null)
            {
                Product = new Product()
                {
                    Id = product.Id,
                    ProductNr = product.ProductNr,
                    Name = product.Name,
                    Price = product.Price,
                };
                ProductNr = product.ProductNr;
                Name = product.Name;
                Price = product.Price.ToString();
            }
            else
            {
                isCreate = true;
            }
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult> { ValidationResult.Success };
        }

        private ICommand _cmdSaveCommand;

        public ICommand CmdSaveCommand
        {
            get
            {
                if (_cmdSaveCommand == null)
                {
                    _cmdSaveCommand = new RelayCommand(
                        execute: async _ =>
                        {
                            ValidateViewModelProperties();

                            try
                            {
                                await using IUnitOfWork uow = new UnitOfWork();

                                if (!isCreate)
                                {
                                    Product productInDb = await uow.Products.GetByIdAsync(Product.Id);
                                    productInDb.ProductNr = ProductNr;
                                    productInDb.Name = Name;
                                    productInDb.Price = Convert.ToDouble(Price);
                                    uow.Products.Update(productInDb);
                                }
                                else
                                {
                                    Product = new Product()
                                    {
                                        ProductNr = ProductNr,
                                        Name = Name,
                                        Price = Convert.ToDouble(Price),
                                    };

                                    await uow.Products.AddAsync(Product);
                                }
                                await uow.SaveChangesAsync();
                                Controller.CloseWindow(this);
                            }
                            catch (ValidationException ex)
                            {
                                if (ex.Value is IEnumerable<string> properties)
                                {
                                    foreach (var property in properties)
                                    {
                                        Errors.Add(property, new List<string> { ex.ValidationResult.ErrorMessage });
                                    }
                                }
                                else
                                {
                                    DbError = ex.ValidationResult.ToString();
                                }
                            }

                        },
                        canExecute: _ => !HasErrors
                        );
                }

                return _cmdSaveCommand;
            }
            set => _cmdSaveCommand = value;
        }

        private ICommand _cmdCancelCommand;

        public ICommand CmdCancelCommand
        {
            get
            {
                if (_cmdCancelCommand == null)
                {
                    _cmdCancelCommand = new RelayCommand(
                        execute: _ =>
                        {
                            Controller.CloseWindow(this);
                        },
                        canExecute => true
                        );
                }

                return _cmdCancelCommand;
            }
            set => _cmdCancelCommand = value;
        }
    }
}
