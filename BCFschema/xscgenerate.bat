::
:: Convert XSD Schema defition files in C# Classes
::
:: -n xxx=ccc map an XML to a C# namespace
:: --dc       disable comments
:: --cn       compact type names
:: -v         verbose
:: -o xxx     define the output directory
::

xscgen --cn --dc -v -n =BCFversion version.xsd
xscgen --cn --dc -v -n =BCFproject project.xsd
xscgen --cn --dc -v -n =BCFmarkup  markup.xsd
xscgen --cn --dc -v -n =BCFvisinfo visinfo.xsd
