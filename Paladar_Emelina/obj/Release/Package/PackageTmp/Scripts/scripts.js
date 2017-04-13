function set_idioma(idioma, url)
{
    $.ajax({
        url: url,
        method: "POST",
        data: { lenguaje: idioma },
        success: function (data)
        {
            location.reload(true);
        }
    });
}