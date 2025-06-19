package plugins.resxformatter.rider

import com.jetbrains.rider.settings.simple.SimpleOptionsPage

class ResxFormatterOptionsPage
    : SimpleOptionsPage("Resx Formatter", "ResxFormatterOptionsPage") {

    override fun getId(): String = pageId + "Id"
}