<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:we="http://www.axa.fr/WebEngine/2022">
    <xsl:template name="content-tab-information-template">
        <xsl:param name="id"/>
        <div id="content-id-information-{$id}" class="tab-content-container class-container-tab" style="display:none">
            <h6>Information : </h6>
            <p>Start time :
                <xsl:value-of select="we:StartTime"/>
            </p>
            <p>End time :
                <xsl:value-of select="we:EndTime"/>
            </p>

            <h6>Screenshot : </h6>
            <div class="container-img">
                <xsl:choose>
                    <xsl:when test="we:Screenshots">
                        <xsl:for-each select="we:Screenshots/we:ScreenshotReport">
                            <xsl:variable name="idImg" select="we:Id"/>
                            <img src="assets/img/{$idImg}.jpg" id="{$idImg}-jpg" class="img-thumbnail img-screenshot" onclick="displayImage('{$idImg}-jpg')"/>
                        </xsl:for-each>
                    </xsl:when>
                </xsl:choose>
            </div>
        </div>
    </xsl:template>
</xsl:stylesheet>