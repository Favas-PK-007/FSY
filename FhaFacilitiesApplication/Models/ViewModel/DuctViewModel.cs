using FhaFacilitiesApplication.Domain.Models.DomainModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class DuctViewModel
    {
        public Guid? ParentGuid { get; set; }
        public string DisplayName { get; set; }
        public int UniqueId { get; set; }
        public Guid UniqueGuid { get; set; }
        [Required(ErrorMessage = "Duct ID(1) is required.")]
        public string DuctId { get; set; }
        [Required(ErrorMessage = "Duct ID(2) is required.")]
        public string DuctIdb { get; set; }
        public Guid? DuctTypeGuid { get; set; }
        public string DuctType { get; set; }
        public string? Comments { get; set; }
        public Guid ConduitGuid { get; set; }
        public List<DuctViewModel> SubDuctListRequest { get; set; }
        public MaterialViewModel Material { get; set; }
        public StructureViewModel Structure { get; set; }
        public string StructureA { get; set; }
        public string StructureB { get; set; }
        public Guid StructureAGuid { get; set; }
        public Guid StructureBGuid { get; set; }

        public List<MaterialTypeViewModel> DuctTypes { get; set; }


        public List<SelectListItem> DuctTypesList { get; set; }
        public List<MaterialViewModel> SubDucts { get; set; }
        public List<SubDuctItem> SubDuctList { get; set; }

        public static DuctViewModel FromModel(DuctModel model)
        {
            var materialVm = model.Material != null ? MaterialViewModel.FromModel(model.Material) : null;

            return new DuctViewModel
            {
                DuctId = model.DuctID,
                DuctIdb = model.DuctID_B,
                UniqueId = model.UniqueID,
                DuctType = materialVm.ModelID,
                UniqueGuid = model.UniqueGUID,
                Comments = model.Comments,
                Material = materialVm,
                Structure = model.Structure != null ? StructureViewModel.FromModel(model.Structure) : null,
                DisplayName = model.ToString()
            };
        }

        public static List<DuctViewModel> FromModelList(List<DuctModel> models)
        {
            return models.Select(FromModel).ToList();
        } 
        
        public static List<MaterialTypeViewModel> FromMaterialTypeModel(List<MaterialTypeModel> models)
        {
            return models.Select(m => new MaterialTypeViewModel
            {
                Text = m.Text,
                Value = m.Value
            }).ToList();
        }

        public class SubDuctItem
        {
            public Guid Id { get; set; }
            public string Label { get; set; }
        }

        public DuctModel ToModel()
        {
            return new DuctModel
            {
                UniqueGUID = UniqueGuid,
                ConduitGUID = ConduitGuid,
                TypeGUID = (Guid)DuctTypeGuid,
                LatestRev = true,
                DuctID = DuctId,
                DuctID_B = DuctIdb,
                Comments = Comments,
                UniqueID = UniqueId,
                
                //SubDucts = SubDucts.Select(sd => sd.ToModel()).ToList(),
            };
        }
    }

    public class MaterialTypeViewModel
    {
        public string Text { get; set; }
        public Guid Value { get; set; }
    }
}
