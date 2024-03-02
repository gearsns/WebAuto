{
    const url = document.location.href
    if (url.includes("lightning.force.com"))
    {
        try {
            const script = document.createElement("script")
            script.src = `https://gears-webauto.lightning.vf.force.com/extract.js`
            document.body.append(script)
        } catch (e) {
            console.log(e)
        }
    }
}
