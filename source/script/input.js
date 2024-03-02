if (document.location.href.includes("-lmis-sd.lightning.force.com")
    || document.location.href.includes("login.salesforce.com")
    || document.location.href.includes("-lmis-sd.my.salesforce.com")
    || document.location.href.includes("-lmis-sd.my.salesforce-sites.com")) {

} else {
    WebAutoCancel = () => { }
    WebAutoSetValue = async (key, value) => {
        try {
            const e = docuemnt.querySelector(`${key},*[name='${key}']`)
            if (!e) {
                throw ""
            }
            else if (e.type == 'checkbox') {
                e.checked = value.length > 0
            }
            else if (e.value !== undefined) {
                e.value = value
            }
            e.focus()
            e.blur()
            e.onchange()
        } catch {

        } finally {
            chrome.webview.hostObjects.WebAuto.CompletedScript()
        }
    }
}