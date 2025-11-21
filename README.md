# Brimborium.Schulaufgaben

Brimborium.Schulaufgaben 

# Architecture

## Editor (Author)

Runs on the author's machine.
Edits the documents.
Publishes the documents to another folder.
Manually transfered to the client(student) server.

## Client (student)

Runs on a server for the students.

# Features 

- Manage the documents.

# Meanings

- Document = Schulaufgabe 
    a folder with a document.json and media files.

- Media = Image, Video, Audio

- MediaGallery = Folder with media files.
    if used in a document will be copied to the document folder.

## EfCore Migrations Commands

```

dotnet tool update --global dotnet-ef

dotnet ef migrations add xxx

dotnet-ef database update

dotnet-ef migrations has-pending-model-changes

```

```
npm install -g npm-run-all

npm i npm-check-updates -g

ncu -u

npm list package

```
