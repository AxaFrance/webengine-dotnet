<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  
  <xsl:import href="header/header-template.xslt"/>
  <xsl:import href="header/sub-header-template.xslt"/>
  <xsl:import href="panel-left/panel-left-template.xslt"/>
  <xsl:import href="panel-right/panel-right-template.xslt"/>

  <xsl:template match="/">
    <html lang="en">
      <head>
        <title>WebEngine Report</title>
        <meta charset="utf-8" />
        <link href="assets/css/global.css" rel="stylesheet" />
        <link href="assets/css/header.css" rel="stylesheet" />
        <link href="assets/css/table.css" rel="stylesheet" />
        <link href="assets/css/badge.css" rel="stylesheet" />
        <link href="assets/css/banner-information.css" rel="stylesheet" />
        <link href="assets/css/body-content.css" rel="stylesheet" />
        <link href="assets/css/tree.css" rel="stylesheet" />
        <link href="assets/css/content-view.css" rel="stylesheet" />
        <link href="assets/css/tab.css" rel="stylesheet" />
        <link href="assets/css/modal.css" rel="stylesheet" />
        <script src="assets/js/global.js"></script>
        <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css" integrity="sha384-MCw98/SFnGE8fJT3GXwEOngsV7Zt27NXFoaoApmYm81iuXoPkFOJwJ8ERdknLPMO" crossorigin="anonymous"/>
        <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css" integrity="sha384-wvfXpqpZZVQGK6TAh5PVlGOfQNHSoD2xbE+QkPxCAFlNEevoEH3Sl0sibVcOQVnN" crossorigin="anonymous"/>
      </head>

      <body>
        <div id="root">
          <div class="site-header common-font">
            <xsl:call-template name="header-template"/>
          </div>

          <div class="general-information scrollbar">
            <xsl:call-template name="sub-header-template"/>
          </div>

          <div class="body-content-container">
            <div id='id-body-left-container' class="body-left-container">
              <xsl:call-template name="tree-view-template"/>
            </div>

            <xsl:call-template name="content-view-template"/>
          </div>
        </div>

        <div id="myModal" class="modal">
          <span class="close">X</span>
          <img class="modal-content" id="img01"/>
          <div id="caption"></div>
        </div>

        <script src="https://code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous" />
        <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js" integrity="sha384-ZMP7rVo3mIykV+2+9J3UJ46jBk0WLaUAdn689aCwoqbBJiSnjAK/l8WvCWPIPm49" crossorigin="anonymous" />
        <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/js/bootstrap.min.js" integrity="sha384-ChfqqxuZUCnJSK3+MXmPNIyE6ZbWh2IMqE241rYiqJxyMiZ6OW/JmZQ5stwEULTy" crossorigin="anonymous" />
        <script>
          openSelectedLineInTree();
          hideElement("body-right-container-class");
        </script>
      </body>
    </html>
  </xsl:template>

</xsl:stylesheet>