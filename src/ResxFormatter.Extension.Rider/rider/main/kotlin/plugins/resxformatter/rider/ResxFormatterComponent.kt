package plugins.resxformatter.rider

import com.intellij.openapi.application.ApplicationManager
import com.intellij.openapi.command.WriteCommandAction
import com.intellij.openapi.editor.Document
import com.intellij.openapi.fileEditor.FileDocumentManagerListener
import com.intellij.openapi.project.Project
import com.intellij.psi.PsiDocumentManager
import com.jetbrains.rd.ide.model.RdResxFormatterFormattingRequest
import com.jetbrains.rd.ide.model.resxFormatterModel
import com.jetbrains.rd.platform.util.idea.ProtocolSubscribedProjectComponent
import com.jetbrains.rd.util.reactive.adviseOnce
import com.jetbrains.rider.ideaInterop.fileTypes.resx.ResxFileLanguage
import com.jetbrains.rider.projectView.solution

class ResxFormatterComponent(project: Project)
    : ProtocolSubscribedProjectComponent(project), FileDocumentManagerListener {

    private val model = project.solution.resxFormatterModel
    private val messageBus = ApplicationManager.getApplication().messageBus.connect()

    init {
        // In Rider, documents are saved in the front-end. Since we run Resx Formatter in the R# backend,
        // we'll need to subscribe to document sync events and piper the document through the backend
        // before save.
        messageBus.subscribe(FileDocumentManagerListener.TOPIC, this)
    }

    override fun beforeDocumentSaving(document: Document) {

        val psiFile = PsiDocumentManager.getInstance(project).getPsiFile(document) ?: return
        if (psiFile.language != ResxFileLanguage) return

        val filePath = psiFile.virtualFile.path
        val currentDocumentText = document.text

        // Perform reformat on back-end, asynchronously
        model.performReformat.start(projectComponentLifetime, RdResxFormatterFormattingRequest(filePath, currentDocumentText)).result
                .adviseOnce(projectComponentLifetime) {
                    val result = it.unwrap()

                    // Only update if backend actually made modifications
                    if (result.isSuccess && result.hasUpdated) {
                        WriteCommandAction.runWriteCommandAction(project) {
                            document.replaceString(0, document.textLength, result.formattedText)
                        }
                    }
                }
    }

    override fun dispose() {
        messageBus.disconnect()
        super.dispose()
    }
}
