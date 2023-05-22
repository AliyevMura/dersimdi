let dbServiceCount = $("#dbServiceCount").val()

$("#btnLoadMore").on("click", () => {
    let serviceCount = $("#services").children().Lenght
    console.log(serviceCount)
    //fetch("/Services/LoadMore")
    //    .then(res => res.text())
    //    .then(data => {
    //        $("#services").append(data)
    //    })
    $.ajax("/Services/LoadMore", {

        method: "GET",
        data: {
            skip: serviceCount,
            take: 8
        },
        success: (data) => {
            $("#services").append(data)
            //serviceCount = $("#services").children().length
            //if (serviceCount >= dbServiceCount) {
            //    $("#btnLoadMore").remove()
            //}
            if (serviceCount >= dbServiceCount) {
                $("#btnLoadMore").remove()
            }
        }
    })
})
