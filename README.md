# HtmlXaml.Core

Converts HTML to FlowDocument.


## Support

This project will currently convert the following HTML tags:-

| category             | tag                           | category  | tag               |
|----------------------|-------------------------------|-----------|-------------------|
| thematic break       | `<hr>`                        | bold      | `<strong>`, `<b>` |
| heading              | `<h1>`, `<h2>`, `<h3>`,       | code span | `<code>`          |
|                      | `<h4>`, `<h5>`, `<h6>`        | hyperlink | `<a>`             |
| indented code blocks | `<pre><code class="lang-**">` | image     | `<img>`           |
| fenced code blocks   | `<pre><code>`                 | italic    | `<em>`, `<i>`     |
| paragraph            | `<p>`                         | linebreak | `<br>`            |
| block quote          | `<blockquote>`                |
| list                 | `<ul>`, `<ol>`                |


And extensions add

| category        | tag                                | category      | tag      |
|-----------------|------------------------------------|---------------|----------|
| citation        | `<cite>`                           | underline     | `<ins>`  |
| figure          | `<figure>`                         | strikethrough | `<del>`  |
| footer          | `<footer>`                         | subscript     | `<sub>`  |
| pipe table      | `<table>`                          | superscript   | `<sup>`  |
| grid table      | `<table>`                          | marked text   | `<mark>` |

## License

This project is licensed under [Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0).