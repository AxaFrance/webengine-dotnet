<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:we="http://www.axa.fr/WebEngine/2022">
    <xsl:template name="content-tab-log-template">
        <xsl:param name="id"/>
        <div id="content-id-log-{$id}" class="tab-content-container class-container-tab" style="display:none">
            <p>
                <xsl:value-of select="we:Log"/>
            </p>
        </div>
    </xsl:template>
</xsl:stylesheet>