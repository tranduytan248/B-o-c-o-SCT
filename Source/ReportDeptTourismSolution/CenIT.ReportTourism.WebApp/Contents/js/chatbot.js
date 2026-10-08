(function () {
    "use strict";
    var host = document.getElementById("sct-chatbot");
    if (!host) return;
    var loader = null;
    var expiryTimer = null;
    var controller = null;
    var stopped = false;
    var generation = 0;

    function destroy() {
        generation += 1;
        window.clearTimeout(expiryTimer);
        if (controller) controller.abort();
        if (loader) {
            window.dispatchEvent(new CustomEvent("vnpt-chat-widget:destroy", { detail: { scriptSrc: loader.src } }));
            loader.remove();
            loader = null;
        }
    }

    async function start() {
        destroy();
        if (stopped) return;
        var currentGeneration = generation;
        var sessionController = new AbortController();
        controller = sessionController;
        var timeout = window.setTimeout(function () { sessionController.abort(); }, 12000);
        try {
            var response = await fetch(host.dataset.sessionUrl, {
                method: "POST",
                credentials: "same-origin",
                cache: "no-store",
                headers: {
                    "Accept": "application/json",
                    "X-CSRF-Token": host.querySelector('input[name="__RequestVerificationToken"]').value
                },
                signal: sessionController.signal
            });
            if (!response.ok) throw new Error("Chatbot session could not be created.");
            var session = await response.json();
            var remaining = Date.parse(session.expiresAt) - Date.now();
            if (!session.viewerToken || !Number.isFinite(remaining) || remaining <= 0) throw new Error("Invalid Chatbot session.");
            if (stopped || currentGeneration !== generation) return;
            loader = document.createElement("script");
            loader.type = "module";
            // Re-evaluate the module for every new viewer session; never put tokens in URLs or storage.
            var url = new URL(host.dataset.loaderUrl);
            url.searchParams.set("sctSession", String(Date.now()));
            loader.src = url.href;
            loader.async = true;
            loader.dataset.viewerToken = session.viewerToken;
            loader.onerror = function () { destroy(); console.warn("Chatbot widget could not be loaded."); };
            document.body.appendChild(loader);
            expiryTimer = window.setTimeout(start, remaining);
        } catch (error) {
            if (error.name !== "AbortError") console.warn("Chatbot is currently unavailable.");
        } finally {
            window.clearTimeout(timeout);
        }
    }

    document.addEventListener("click", function (event) {
        var anchor = event.target.closest && event.target.closest("a[href]");
        if (!anchor) return;
        var url = new URL(anchor.href, window.location.href);
        if (url.origin === window.location.origin && /\/Account\/Logout\/?$/i.test(url.pathname)) {
            stopped = true;
            destroy();
            try { localStorage.setItem("sct-chatbot-logout", String(Date.now())); } catch (error) { /* Storage may be disabled. */ }
        }
    });
    window.addEventListener("storage", function (event) {
        if (event.key === "sct-chatbot-logout") { stopped = true; destroy(); }
    });
    window.addEventListener("pagehide", function () { stopped = true; destroy(); });
    window.addEventListener("pageshow", function (event) {
        if (event.persisted) { stopped = false; start(); }
    });
    start();
}());
