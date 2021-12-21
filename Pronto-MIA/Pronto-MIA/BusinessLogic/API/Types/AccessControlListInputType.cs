namespace Pronto_MIA.BusinessLogic.API.Types
{
    using System.Diagnostics.CodeAnalysis;
    using HotChocolate.Types;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing the input type of a <see cref="AccessControlList"/>
    /// object. This class is used to determine which fields are necessary
    /// to create a <see cref="AccessControlList"/> input.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AccessControlListInputType : InputObjectType<AccessControlList>
    {
        /// <inheritdoc/>
        [SuppressMessage(
        "Menees.Analyzers",
        "MEN003",
        Justification = "Many Access Controls")]
        protected override void Configure(
            IInputObjectTypeDescriptor<AccessControlList> descriptor)
        {
            IgnoreFields(descriptor);
            descriptor.Field(t => t.CanEditUsers)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanEditDepartmentUsers)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanViewUsers)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanViewDepartmentUsers)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanEditDepartments)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanEditOwnDepartment)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanViewDepartments)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanViewOwnDepartment)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanEditDeploymentPlans)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanEditDepartmentDeploymentPlans)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanViewDeploymentPlans)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanViewDepartmentDeploymentPlans)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanViewExternalNews)
                .Type<BooleanType>().DefaultValue(true);
            descriptor.Field(t => t.CanEditExternalNews)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanViewInternalNews)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanEditInternalNews)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanViewEducationalContent)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanEditEducationalContent)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanViewAppointment)
                .Type<BooleanType>().DefaultValue(false);
            descriptor.Field(t => t.CanEditAppointment)
                .Type<BooleanType>().DefaultValue(false);
        }

        private static void IgnoreFields(
            IInputObjectTypeDescriptor<AccessControlList> descriptor)
        {
            descriptor.Field(t => t.Id).Ignore();
            descriptor.Field(t => t.UserId).Ignore();
            descriptor.Field(t => t.User).Ignore();
        }
    }
}
