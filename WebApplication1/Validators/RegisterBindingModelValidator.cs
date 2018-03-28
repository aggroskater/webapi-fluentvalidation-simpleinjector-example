using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Validators {
    public class RegisterBindingModelValidator : AbstractValidator<RegisterBindingModel> {

        public RegisterBindingModelValidator() {
            RuleFor(p => p.Email)
                .NotNull()
                .NotEmpty()
                .Equal("lolwut");
        }

    }
}