# PIP — Plastic Is Pollution: Petition Website

A simple static website for the PIP petition. No build step, no dependencies —
just open `index.html` in a browser.

## Hosting on GitHub Pages

This folder lives inside `docs/`, which is the source of this repository's
GitHub Pages site. Once these files are on the `main` branch and the `pages`
workflow has run (GitHub → Actions → "pages" → "Run workflow"), the petition
site is served at:

**https://devodo.github.io/DivertR/pip/**

The rest of the Pages site (the DivertR library docs) is unaffected — Jekyll
copies these plain HTML/CSS/JS files through as-is.

## Pages

| Page | File | Purpose |
|---|---|---|
| Home | `index.html` | Landing page with the campaign headline and key stats |
| About | `about.html` | Who PIP is, the campaign's story and demands |
| Sign the Petition | `sign.html` | Signature form + the petition document (table, CSV download, print view) |
| What We're Doing | `what-we-are-doing.html` | Current, upcoming and completed campaign activity |

## Editing the content

All the text is placeholder copy you can rewrite. Search the HTML files for
`EDIT ME` comments — each one marks a block of copy intended to be replaced:

- Hero headline and intro (`index.html`)
- Campaign stats (`index.html` — replace with sourced figures)
- Your story and demands (`about.html`)
- The petition statement (`sign.html`)
- Campaign activity cards (`what-we-are-doing.html`)
- Contact email in every footer (currently `contact@example.org`)

Colours and fonts live at the top of `css/styles.css` in the `:root` block.

## How signatures work

Signatures submitted on the Sign page are:

1. Saved in the visitor's browser (localStorage) and shown in the
   "Petition document" table on the page,
2. Downloadable as a CSV spreadsheet,
3. Printable as a clean petition document (the print view hides the
   navigation and form, leaving just the petition text and signature table).

**Limitation:** localStorage is per-browser, so each visitor only sees
signatures collected on their own device. To collect all signatures into one
shared document, connect a backend — the easiest option is a Google Form
linked to a Google Sheet. Instructions are in the comments at the top of
`js/petition.js` (fill in the form URL and field IDs, then set
`BACKEND_ENABLED = true`).
