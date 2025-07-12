package plugins.resxformatter.rider

import com.intellij.openapi.command.WriteCommandAction
import com.intellij.openapi.editor.Document
import com.intellij.openapi.fileEditor.FileDocumentManagerListener
import com.intellij.openapi.project.Project
import com.intellij.openapi.rd.createLifetime
import com.intellij.openapi.util.Disposer
import com.intellij.psi.PsiDocumentManager
import com.jetbrains.rd.ide.model.RdResxFormatterFormattingRequest
import com.jetbrains.rd.ide.model.resxFormatterModel
import com.jetbrains.rd.util.reactive.adviseOnce
import com.jetbrains.rider.ideaInterop.fileTypes.resx.ResxFileLanguage
import com.jetbrains.rider.projectView.solution

class ResxFormatterListener private constructor(private val project: Project)
    : FileDocumentManagerListener {

    private val model = project.solution.resxFormatterModel

    override fun beforeDocumentSaving(document: Document) {
        val psiFile = PsiDocumentManager.getInstance(project).getPsiFile(document) ?: return
        if (psiFile.language != ResxFileLanguage) return

        val filePath = psiFile.virtualFile.path
        val currentDocumentText = document.text

        // Perform reformat on back-end, asynchronously
        val disposable = Disposer.newDisposable("ResxFormatterComponent Disposable")
        val lifetime = disposable.createLifetime()
        model.performReformat.start(lifetime, RdResxFormatterFormattingRequest(filePath, currentDocumentText)).result
            .adviseOnce(lifetime) {
                val result = it.unwrap()

                // Only update if backend actually made modifications
                if (result.isSuccess && result.hasUpdated) {
                    WriteCommandAction.runWriteCommandAction(project) {
                        document.replaceString(0, document.textLength, result.formattedText)
                    }
                }
                Disposer.dispose(disposable)
            }
    }
}
