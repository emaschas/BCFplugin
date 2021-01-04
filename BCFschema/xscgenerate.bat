::
:: Convert XSD Schema defition files in C# Classes
::
:: -n xxx=ccc map an XML to a C# namespace
:: --dc       disable comments
:: -v         verbose
:: -o xxx     define the output directory
::

xscgen --dc -v -n =BCFversion version.xsd
xscgen --dc -v -n =BCFproject project.xsd
xscgen --dc -v -n =BCFmarkup  markup.xsd
xscgen --dc -v -n =BCFvisinfo visinfo.xsd
