<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:we="http://www.axa.fr/WebEngine/2022">

    <xsl:template name="tab-action-template">
        <xsl:param name="id"/>
        <div class="tab-bar">
            <div id="tab-id-information-{$id}" class="tab-label tablink tab-bottombar w3-hover-light-grey w3-padding"
                 onclick="openSelectedTab('tab-id-information-{$id}', 'content-id-information-{$id}');">
                <a href="#"
                   onclick="openSelectedTab('tab-id-information-{$id}', 'content-id-information-{$id}');">
                    <label class="label-in-tab">Information / Screenshot</label>
                </a>
            </div>
            <div id="tab-id-log-{$id}" class="tab-label tablink tab-bottombar w3-hover-light-grey w3-padding"
                 onclick="openSelectedTab('tab-id-log-{$id}', 'content-id-log-{$id}');">
                <a href="#" onclick="openSelectedTab('tab-id-log-{$id}', 'content-id-log-{$id}');">
                    <label class="label-in-tab">Log</label>
                </a>
            </div>
            <div id="tab-id-context-value-{$id}" class="tab-label tablink tab-bottombar w3-hover-light-grey w3-padding"
                 onclick="openSelectedTab('tab-id-context-value-{$id}', 'content-id-context-value-{$id}');">
                <a href="#"
                   onclick="openSelectedTab('tab-id-context-value-{$id}', 'content-id-context-value-{$id}');">
                    <label class="label-in-tab">Context value</label>
                </a>
            </div>
        </div>
    </xsl:template>
</xsl:stylesheet>