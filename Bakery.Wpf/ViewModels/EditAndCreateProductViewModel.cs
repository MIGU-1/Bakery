using Bakery.Core.Entities;
using Bakery.Wpf.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Bakery.Wpf.ViewModels
{
    class EditAndCreateProductViewModel : BaseViewModel
    {
        public EditAndCreateProductViewModel(IWindowController controller, Product product):base(controller)
        {

        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
