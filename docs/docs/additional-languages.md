# Additional Languages

Docfx natively supports the generation of reference documentation for .NET and REST API. However, if you require support for other languages, you will need to create a custom API docs converter tailored to the language of your choice. An API docs converter is a tool that operates prior to the execution of docfx and generates files that are compatible with the docfx build process. An instance of an API docs converter is the `docfx metadata` command utilized for .NET.

The output files generated by an API docs converter typically include:

1. A `toc.yml` file: This file represents the generated hierarchy in the table of content section of the page. For detailed information regarding the data structure of a TOC file, refer to the [Table of Content](./table-of-contents.md) documentation.

2. A series of YAML files: These adhere to the [API Page](./api-page.yml) structure, each containing an individual page. The file path of the generated page corresponds to the URL used to access the page. For comprehensive documentation on the API reference page data structure, refer to the [API Page](./api-page.yml).

To utilize the artifact generated by an API converter, you need to modify the `docfx.json` configuration file to incorporate the artifact as a build input. For instance, consider the following directory structure:

```csharp
|- docfx.json
|- api // <-- output directory of custom API converter
    |- toc.yml
    |- api-page-1.yml
    |- api-page-2.yml
```

In the above structure, you can configure `docfx.json` to include the output of the API converter in the following manner:

```json
{
  "build": {
    "content": [
      { "files": "api/**/*.yml" }
    ],
  }
}
```