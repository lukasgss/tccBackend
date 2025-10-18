using Application.Common.DTOs;
using Domain.Enums;

namespace Infrastructure.SharingAlerts;

public record AdoptionAlertPrintData(
    string Base64Image,
    string PetName,
    AgeResponse Age,
    string Size,
    GenderResponse Gender,
    bool? IsCastrated,
    string ContactName,
    string ContactPhoneNumber);

public static class DownloadTemplates
{
    public static string GetShareAdoptionPdfTemplate(AdoptionAlertPrintData alertPrintData)
    {
        // Minified for performance reasons
        return
            $$"""<!doctypehtml><html lang=pt-BR><meta charset=UTF-8><meta content="IE=edge"http-equiv=X-UA-Compatible><meta name=viewport width="device-width, initial-scale=1.0"><title>Adote {{alertPrintData.PetName}}</title><link href=https://fonts.googleapis.com rel=preconnect><link href=https://fonts.gstatic.com rel=preconnect crossorigin><link href="https://fonts.googleapis.com/css2?family=Caveat:wght@700&family=Merienda:wght@400;700&family=Itim&family=Dancing+Script:wght@400..700&display=swap"rel=stylesheet><style>body,html{margin:0;padding:0;font-family:Itim,cursive;box-sizing:border-box}.flyer{width:100%;display:flex;flex-direction:column;justify-content:space-between;align-items:center;box-sizing:border-box}.adote{font-family:"Dancing Script",cursive;font-weight:700;font-size:140px;color:#4169e1;text-align:center;margin:0;margin-top:-50px;position:relative;z-index:2}.image-container{display:flex;flex-direction:column;align-items:center}.dog-image{width:400px;height:400px;border-radius:50%;object-fit:cover;border:3px solid orange}.trait{background-color:orange;color:#fff;padding:10px 20px;text-align:center;border-radius:20px;font-size:40px;font-weight:700;white-space:nowrap;z-index:3;margin-top:-20px}.name{font-family:Merienda,cursive;font-size:60px;color:#8b4513;text-align:center;margin:15px 0 0;font-weight:700;position:relative;z-index:2}.details{font-size:44px;color:#d08855;text-align:center;margin-bottom:10px;font-weight:400;position:relative;z-index:2}.contact{font-size:44px;color:#4169e1;text-align:center;margin-top:5px;font-weight:700;position:relative;z-index:2}</style><div class=flyer><h1 class=adote>Adote</h1><div class=image-container><img class=dog-image src=data:image/jpeg;base64,{{alertPrintData.Base64Image}}><div class=trait>EM BUSCA DE UM LAR</div></div><div class=name>{{alertPrintData.PetName.ToUpperInvariant()}}</div><div class=details>{{GetAgeText(alertPrintData.Age, alertPrintData.Gender)}}<br>Porte {{alertPrintData.Size.ToLowerInvariant()}}<br>{{GetGenderText(alertPrintData.Gender, alertPrintData.IsCastrated)}}</div><div class=contact>{{alertPrintData.ContactName}}</div><div class=contact>{{alertPrintData.ContactPhoneNumber}}</div></div>""";
    }

    private static string GetAgeText(AgeResponse age, GenderResponse gender)
    {
        return age.Id == Age.Adulto && gender.Id == Gender.Fêmea ? "Adulta" : "Adulto";
    }

    private static string GetGenderText(GenderResponse sex, bool? isCastrated)
    {
        if (isCastrated is not null && isCastrated.Value)
        {
            return sex.Id == Gender.Fêmea ? $"{sex.Name} castrada" : $"{sex.Name} castrado";
        }

        return sex.Name;
    }
}