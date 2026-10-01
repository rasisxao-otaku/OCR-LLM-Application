from fastapi import FastAPI, UploadFile, File
from paddleocr import PaddleOCR
from pdf2image import convert_from_path

from PIL import ImageEnhance, ImageFilter

import tempfile
import os
import unicodedata
import json
import uuid


app = FastAPI()


ocr = PaddleOCR(
    lang="japan",
    use_angle_cls=True
)


# OCR誤認識補正辞書
CORRECTION_DICT = {

    # 日本語誤認識
    "車休": "車体",
    "部島": "部品",
    "郊品": "部品",

    "酉含率": "配合率",
    "新色酉含率": "新色配合率",

    "廃上": "廃止",
    "望装": "塗装",
    "金装": "塗装",

    "統-": "統一",

    # 車種関連
    "セルボモート": "セルボモード",
    "アルトノ": "アルト",

    # メーカー
    "SUZUK": "SUZUKI",
    "SUlUK!": "SUZUKI",
    "スズキ": "SUZUKI",

    # 部品名称
    "サ一一部品": "サービス部品",
    "サービス郊品": "サービス部品",
}


def normalize_text(text: str):

    return unicodedata.normalize(
        "NFKC",
        text
    )


def correct_text(text: str):

    text = normalize_text(text)

    for before, after in CORRECTION_DICT.items():

        text = text.replace(
            before,
            after
        )

    return text



@app.post("/ocr")
async def ocr_pdf(
    file: UploadFile = File(...)
):

    temp_pdf = tempfile.NamedTemporaryFile(
        delete=False,
        suffix=".pdf"
    )


    temp_pdf.write(
        await file.read()
    )

    temp_pdf.close()


    image_files = []


    try:

        #
        # PDF全ページ画像化
        #
        images = convert_from_path(
            temp_pdf.name,
            dpi=300
        )


        pages = []


        for index, image in enumerate(images):


            image_path = (
                f"/tmp/"
                f"ocr_{uuid.uuid4().hex}.png"
            )


            #
            # 前処理
            #

            image = image.convert(
                "L"
            )


            image = ImageEnhance.Contrast(
                image
            ).enhance(
                2.0
            )


            image = image.filter(
                ImageFilter.SHARPEN
            )


            image.save(
                image_path
            )


            image_files.append(
                image_path
            )


            #
            # OCR
            #

            result = ocr.ocr(
                image_path,
                cls=True
            )


            lines = []
            page_text = []


            if result and result[0]:

                for item in result[0]:

                    bbox = item[0]

                    text = item[1][0]

                    confidence = item[1][1]


                    corrected = correct_text(
                        text
                    )


                    page_text.append(
                        corrected
                    )


                    lines.append(
                        {
                            "text": text,

                            "confidence":
                                round(
                                    confidence,
                                    3
                                ),

                            "corrected":
                                corrected,

                            "bbox":
                                bbox,

                            "review_required":
                                confidence < 0.7
                        }
                    )


            pages.append(
                {
                    "page":
                        index + 1,

                    "full_text":
                        "\n".join(
                            page_text
                        ),

                    "lines":
                        lines
                }
            )


            print(
                f"PAGE {index+1} OCR COMPLETE "
                f"LINES={len(lines)}"
            )


        result_json = {

            "filename":
                file.filename,

            "page_count":
                len(pages),

            "pages":
                pages
        }


        #
        # JSON保存
        #

        os.makedirs(
            "ocr_results",
            exist_ok=True
        )


        output_file = os.path.join(
            "ocr_results",
            f"{os.path.splitext(file.filename)[0]}.json"
        )


        with open(
            output_file,
            "w",
            encoding="utf-8"
        ) as f:

            json.dump(
                result_json,
                f,
                ensure_ascii=False,
                indent=2
            )


        return result_json



    finally:


        if os.path.exists(
            temp_pdf.name
        ):

            os.remove(
                temp_pdf.name
            )


        for img in image_files:

            if os.path.exists(
                img
            ):

                os.remove(
                    img
                )