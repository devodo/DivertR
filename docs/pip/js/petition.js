/* ============================================================
   PIP — Plastic Is Pollution
   Petition signing logic.

   How it works today (no server needed):
   - Signatures are saved in the visitor's browser (localStorage)
     and shown in the "Petition document" table on sign.html.
   - "Download as CSV" exports all collected signatures as a
     spreadsheet file.
   - "Print petition document" opens the browser print dialog with
     a clean, document-style layout (the form and nav are hidden).

   IMPORTANT LIMITATION: localStorage is per-browser, so each
   visitor only sees signatures collected on their own device.
   To collect everyone's signatures in one shared document, hook
   up a backend in submitToBackend() below — the easiest option
   is a Google Form that feeds a Google Sheet.
   ============================================================ */

(function () {
  "use strict";

  var STORAGE_KEY = "pip-petition-signatures";

  /* ----------------------------------------------------------
     OPTIONAL BACKEND HOOK
     To send signatures to a shared document (e.g. Google Sheet):
     1. Create a Google Form with fields: name, email, location, comment.
     2. Get the form's action URL and each field's entry ID
        (view the form's page source, or use a "prefilled link").
     3. Fill in the values below and set BACKEND_ENABLED to true.
     Signatures will then be submitted to the form AND still shown
     in the local table.
     ---------------------------------------------------------- */
  var BACKEND_ENABLED = false;
  var GOOGLE_FORM_ACTION = "https://docs.google.com/forms/d/e/YOUR_FORM_ID/formResponse";
  var GOOGLE_FORM_FIELDS = {
    name: "entry.0000001",
    email: "entry.0000002",
    location: "entry.0000003",
    comment: "entry.0000004"
  };

  function submitToBackend(signature) {
    if (!BACKEND_ENABLED) return;
    var data = new FormData();
    data.append(GOOGLE_FORM_FIELDS.name, signature.name);
    data.append(GOOGLE_FORM_FIELDS.email, signature.email);
    data.append(GOOGLE_FORM_FIELDS.location, signature.location);
    data.append(GOOGLE_FORM_FIELDS.comment, signature.comment);
    // no-cors: Google Forms doesn't return a readable response,
    // but the submission still lands in the linked Sheet.
    fetch(GOOGLE_FORM_ACTION, { method: "POST", mode: "no-cors", body: data });
  }

  /* ---------- Local storage helpers ---------- */

  function loadSignatures() {
    try {
      return JSON.parse(localStorage.getItem(STORAGE_KEY)) || [];
    } catch (e) {
      return [];
    }
  }

  function saveSignatures(signatures) {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(signatures));
    } catch (e) {
      /* storage unavailable (private browsing etc.) — table still works for this visit */
    }
  }

  /* ---------- Rendering ---------- */

  var form = document.getElementById("petition-form");
  var message = document.getElementById("form-message");
  var tableBody = document.getElementById("signature-body");
  var emptyNote = document.getElementById("empty-note");
  var countLabel = document.getElementById("signature-count");

  function renderTable() {
    var signatures = loadSignatures();
    tableBody.innerHTML = "";

    signatures.forEach(function (sig, index) {
      var row = document.createElement("tr");
      [index + 1, sig.name, sig.location || "—", sig.comment || "—", sig.date].forEach(function (value) {
        var cell = document.createElement("td");
        cell.textContent = value;
        row.appendChild(cell);
      });
      tableBody.appendChild(row);
    });

    emptyNote.style.display = signatures.length ? "none" : "block";
    countLabel.textContent = signatures.length + (signatures.length === 1 ? " signature" : " signatures");
  }

  function showMessage(text, type) {
    message.textContent = text;
    message.className = "form-message " + type;
  }

  /* ---------- Form submission ---------- */

  form.addEventListener("submit", function (event) {
    event.preventDefault();

    var name = form.name.value.trim();
    var email = form.email.value.trim();
    var location = form.location.value.trim();
    var comment = form.comment.value.trim();

    if (!name || !email) {
      showMessage("Please fill in your name and email address.", "error");
      return;
    }
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      showMessage("Please enter a valid email address.", "error");
      return;
    }
    if (!form.consent.checked) {
      showMessage("Please tick the consent box so we can include your signature in the petition document.", "error");
      return;
    }

    var signatures = loadSignatures();

    var alreadySigned = signatures.some(function (sig) {
      return sig.email.toLowerCase() === email.toLowerCase();
    });
    if (alreadySigned) {
      showMessage("It looks like this email address has already signed — thank you for your support!", "error");
      return;
    }

    var signature = {
      name: name,
      email: email,
      location: location,
      comment: comment,
      date: new Date().toLocaleDateString()
    };

    signatures.push(signature);
    saveSignatures(signatures);
    submitToBackend(signature);
    renderTable();
    form.reset();
    showMessage("Thank you, " + name + "! Your signature has been added to the petition document.", "success");
  });

  /* ---------- Download as CSV ---------- */

  function csvEscape(value) {
    value = String(value == null ? "" : value);
    if (/[",\n]/.test(value)) {
      value = '"' + value.replace(/"/g, '""') + '"';
    }
    return value;
  }

  document.getElementById("download-csv").addEventListener("click", function () {
    var signatures = loadSignatures();
    if (!signatures.length) {
      showMessage("There are no signatures to download yet.", "error");
      return;
    }

    var rows = [["#", "Name", "Email", "Location", "Comment", "Date signed"]];
    signatures.forEach(function (sig, index) {
      rows.push([index + 1, sig.name, sig.email, sig.location, sig.comment, sig.date]);
    });

    var csv = rows.map(function (row) {
      return row.map(csvEscape).join(",");
    }).join("\n");

    var blob = new Blob([csv], { type: "text/csv;charset=utf-8;" });
    var link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = "pip-petition-signatures.csv";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(link.href);
  });

  /* ---------- Print as a formal document ---------- */

  document.getElementById("print-document").addEventListener("click", function () {
    window.print();
  });

  /* ---------- Init ---------- */

  renderTable();
})();
