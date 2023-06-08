<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"  xmlns:we="http://www.axa.fr/WebEngine/2022">

    <xsl:template name="status-template">
        <xsl:param name="status" />
        <xsl:param name="label" />
        <xsl:param name="id" />
        <xsl:if test="$status='Passed'">
            <i class="fa fa-check badge-action-green" aria-hidden="true"></i>
        </xsl:if>
        <xsl:if test="$status='Failed'">
            <i class="fa fa-times badge-action-red" aria-hidden="true"></i>
        </xsl:if>
        <xsl:if test="$status='Ignored'">
            <i class="fa fa-circle badge-action-gray" aria-hidden="true"></i>
        </xsl:if>
        <xsl:text> </xsl:text>
        <a href="#" onclick="changeColorOfSelectedElement(event);hideElement('body-right-container-class');showElement('{$id}');openSelectedTab('tab-id-information-{$id}','content-id-information-{$id}');"><xsl:value-of select="$label" /></a>
    </xsl:template>

</xsl:stylesheet>