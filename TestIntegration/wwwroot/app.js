function setApiKey(apiKey) {
    console.log("get cookie " + document.cookie)
    document.cookie = `apiKey=${apiKey}; path=/; HttpOnly; Secure`;
    localStorage.setItem("apiKey", apiKey)
    console.log("update cookie")
}
//function getApiKey() {
//    return document.cookie.
//}
document.addEventListener("DOMContentLoaded", function () {
    const apiKey = document.getElementById("apiKey")
    if (apiKey) {
        apiKey.value = localStorage.getItem("apiKey")
        apiKey.addEventListener("input", function () {
            setApiKey(apiKey.value);
        });
    }
})