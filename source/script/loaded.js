if (document.location.href.includes("-lmis-sd.lightning.force.com")
    || document.location.href.includes("login.salesforce.com")
    || document.location.href.includes("-lmis-sd.my.salesforce.com")
    || document.location.href.includes("-lmis-sd.my.salesforce-sites.com")) {
    try {
        document.addEventListener("DOMContentLoaded", () => {
            try {
                const script = document.createElement("script")
                script.src = "https://gears-webauto.lightning.vf.force.com/script.js"
                document.body.append(script)
            } catch (e) {
                console.log(e)
            }
        })
    } catch (e) {
        console.log(e)
    }
}