<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:we="http://www.axa.fr/WebEngine/2022">
    <xsl:template name="tab-test-case-template">
        <xsl:param name="id"/>
        <div class="tab-bar">
            <div id="tab-id-information-{$id}" class="tab-label tablink tab-bottombar w3-hover-light-grey w3-padding"
                 onclick="openSelectedTab('tab-id-information-{$id}', 'content-id-information-{$id}');">
                <a href="#"
                   onclick="openSelectedTab('tab-id-information-{$id}', 'content-id-information-{$id}');">
                    <label class="label-in-tab">Information</label>
                </a>
            </div>
            <div id="tab-id-log-{$id}" class="tab-label tablink tab-bottombar w3-hover-light-grey w3-padding"
                 onclick="openSelectedTab('tab-id-log-{$id}', 'content-id-log-{$id}');">
                <a href="#" onclick="openSelectedTab('tab-id-log-{$id}', 'content-id-log-{$id}');">
                    <label class="label-in-tab">Log</label>
                </a>
            </div>
            <div id="tab-id-test-data-{$id}" class="tab-label tablink tab-bottombar w3-hover-light-grey w3-padding"
                 onclick="openSelectedTab('tab-id-test-data-{$id}', 'content-id-test-data-{$id}');">
                <a href="#"
                   onclick="openSelectedTab('tab-id-test-data-{$id}', 'content-id-test-data-{$id}');">
                    <label class="label-in-tab">Test data</label>
                </a>
            </div>
        </div>
    </xsl:template>
</xsl:stylesheet>