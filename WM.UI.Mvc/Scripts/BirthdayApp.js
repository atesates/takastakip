var model = {
    view: ko.observable("hosgeldin"),
    davetiyeModel: {
        Ad: ko.observable(""),
        Eposta: ko.observable(""),
        KatilmaDurumu: ko.observable("true")
    },
    Katilanlar: ko.observableArray([])
}

var formGoster = function () {
    model.view("form");
}
var formuGonder = function () {
    $.ajax("../../../EczaneNobet/EczaneGrupTanim/ExpandDurumuDegistir",
        {
            type: "POST",
            data: {
                Ad: model.davetiyeModel.Ad,
                Eposta: model.davetiyeModel.Eposta,
                KatilmaDurumu: model.davetiyeModel.KatilmaDurumu
            },
                success: function (data) {

                   katilanlarigetir()
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    addmsg("error", textStatus + " (" + errorThrown + ")");
                    alert(textStatus + " (" + errorThrown + ")");
                }
            });

}

var katilanlariGetir = function () {

    $.ajax("../../../EczaneNobet/EczaneGrupTanim/ExpandDurumuDegistir", {
        type: "POST",
        success: function (data) {
            model.Katilanlar.removeAll();
            model.Katilanlar.push.apply(model.Katilanlar, data.map(function (item) {
                return item.Ad;
            }));
            model.view("Thanks");
        }
    });
}
$(document).ready(function () {
    ko.applyBindings();
});