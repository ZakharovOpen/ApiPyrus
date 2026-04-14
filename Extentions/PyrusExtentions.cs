using ApiPyrus.Models.DTOs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ApiPyrus.Extentions
{
    public static class PyrusExtentions
    {
        public static string GetJson<T>(this T entityForJson, Formatting jsonFormatting = Formatting.Indented, NullValueHandling nullValueHandling = NullValueHandling.Ignore, DefaultValueHandling defaultValueHandling = DefaultValueHandling.Ignore, ReferenceLoopHandling referenceLoopHandling = ReferenceLoopHandling.Ignore)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = jsonFormatting,
                NullValueHandling = nullValueHandling,
                DefaultValueHandling = defaultValueHandling,
                ReferenceLoopHandling = referenceLoopHandling
            };

            return JsonConvert.SerializeObject(entityForJson, settings);
        }
        public static string GetJson<T>(this T entityForJson, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(entityForJson, settings);
        }
        public static Field GetFieldById(this PyrusTask task, long fieldId)
           => task.Fields.GetFieldById(fieldId);

        public static Field GetFieldById(this List<Field> fields, long fieldId)
        {
            var field = fields.FirstOrDefault(x => x.Id == fieldId);
            if (field != null)
                return field;
            if (!fields.Any(f => f.GetValueObject() is ValueTitle) && !fields.Any(f => f.GetValueObject() is ValueMultipleChoice)) 
                return null;
            var titlesFields = fields.Where(f => f.GetValueObject() is ValueTitle)?.SelectMany(f => f.GetValue<ValueTitle>().Fields);
            field = titlesFields?.FirstOrDefault(f => f.Id == fieldId);
            if (field != null)
                return field;
            var multipleChoicesFields = fields.Where(f => f.GetValueObject() is ValueMultipleChoice valueMultipleChoice && valueMultipleChoice.Fields != null)?.SelectMany(f => f.GetValue<ValueMultipleChoice>().Fields);
            field = multipleChoicesFields?.FirstOrDefault(f => f.Id == fieldId);
            return field;
        }

        public static bool TryGetFieldById(this PyrusTask task, long fieldId, out Field field)
            => task.Fields.TryGetFieldById(fieldId, out field);

        public static bool TryGetFieldById(this List<Field> fields, long fieldId, out Field field)
        {
            field = fields.FirstOrDefault(x => x.Id == fieldId);
            if (field != null)
                return true;
            if (!fields.Any(f => f.GetValueObject() is ValueTitle) && !fields.Any(f => f.GetValueObject() is ValueMultipleChoice))
                return false;
            var titlesFields = fields.Where(f => f.GetValueObject() is ValueTitle)?.SelectMany(f => f.GetValue<ValueTitle>().Fields);
            field = titlesFields?.FirstOrDefault(f => f.Id == fieldId);
            if (field != null)
                return true;
            var multipleChoicesFields = fields.Where(f => f.GetValueObject() is ValueMultipleChoice valueMultipleChoice && valueMultipleChoice.Fields != null)?.SelectMany(f => f.GetValue<ValueMultipleChoice>().Fields);
            field = multipleChoicesFields?.FirstOrDefault(f => f.Id == fieldId);

            return field != null;
        }

        public static List<Field> GetFieldsByType(this PyrusTask task, FieldTypes type)
           => task.Fields.GetFieldsByType(type);

        public static List<Field> GetFieldsByType(this List<Field> fields, FieldTypes type)
        {
            List<Field> fieldsByType = new List<Field>();
            if (fields.Any(f => f.Type == type))
                fieldsByType = fields.Where(f => f.Type == type).ToList();
            if (fields.Any(f => f.GetValueObject() is ValueTitle))
            {
                var titlesFields = fields.Where(f => f.GetValueObject() is ValueTitle).SelectMany(f => f.GetValue<ValueTitle>().Fields);
                if (titlesFields.Any(f => f.Type == type))
                    fieldsByType.AddRange(titlesFields.Where(f => f.Type == type));
            }
            if (fields.Any(f => f.GetValueObject() is ValueMultipleChoice))
            {
                var multipleChoicesFields = fields.Where(f => f.GetValueObject() is ValueMultipleChoice valueMultipleChoice && valueMultipleChoice.Fields != null)?.SelectMany(f => f.GetValue<ValueMultipleChoice>().Fields);
                if (multipleChoicesFields != null && multipleChoicesFields.Any(f => f.Type == type))
                    fieldsByType.AddRange(multipleChoicesFields.Where(f => f.Type == type));
            }
            return fieldsByType;
        }

        public static bool TryGetFieldsByType(this PyrusTask task, FieldTypes type, out List<Field> fields)
            => task.Fields.TryGetFieldsByType(type, out fields);

        public static bool TryGetFieldsByType(this List<Field> fields, FieldTypes type, out List<Field> fieldsByType)
        {
            fieldsByType = new List<Field>();
            if (fields.Any(f => f.Type == type))
                fieldsByType = fields.Where(f => f.Type == type).ToList();
            if (fields.Any(f => f.GetValueObject() is ValueTitle))
            {
                var titlesFields = fields.Where(f => f.GetValueObject() is ValueTitle).SelectMany(f => f.GetValue<ValueTitle>().Fields);
                if (titlesFields.Any(f => f.Type == type))
                    fieldsByType.AddRange(titlesFields.Where(f => f.Type == type));
            }
            if (fields.Any(f => f.GetValueObject() is ValueMultipleChoice))
            {
                var multipleChoicesFields = fields.Where(f => f.GetValueObject() is ValueMultipleChoice valueMultipleChoice && valueMultipleChoice.Fields != null)?.SelectMany(f => f.GetValue<ValueMultipleChoice>().Fields);
                if (multipleChoicesFields != null && multipleChoicesFields.Any(f => f.Type == type))
                    fieldsByType.AddRange(multipleChoicesFields.Where(f => f.Type == type));
            }
            return fieldsByType.Count > 0;
        }
    }
}
