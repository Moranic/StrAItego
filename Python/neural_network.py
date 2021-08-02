import tensorflow as tf
from tensorflow import keras
from tensorflow.python.framework.convert_to_constants import convert_variables_to_constants_v2

import numpy as np
import os

from StateEvaluator import *

if __name__ == '__main__':
    # LOAD MODEL
    model = tf.keras.Sequential()
    model.add(tf.keras.layers.Dense(300, activation='relu', kernel_initializer=kernel1 ,bias_initializer=bias1))
    model.add(tf.keras.layers.Dense(300, activation='relu', kernel_initializer=kernel2 ,bias_initializer=bias2))
    model.add(tf.keras.layers.Dense(300, activation='relu', kernel_initializer=kernel3 ,bias_initializer=bias3))
    model.add(tf.keras.layers.Dense(300, activation='relu', kernel_initializer=kernel4 ,bias_initializer=bias4))
    model.add(tf.keras.layers.Dense(300, activation='relu', kernel_initializer=kernel5 ,bias_initializer=bias5))
    model.add(tf.keras.layers.Dense(2,                      kernel_initializer=kernel6 ,bias_initializer=bias6))

    #DRE model
    #inputs = tf.keras.Input(shape=(480))
    #layer11    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel1    ,bias_initializer=bias1    ) (inputs)
    #layer12    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel2    ,bias_initializer=bias2    ) (layer11)
    #layer13    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel3    ,bias_initializer=bias3    ) (layer12)
    #layer14    = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel4    ,bias_initializer=bias4    ) (layer13)
    #layer21    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel5    ,bias_initializer=bias5    ) (inputs)
    #layer22    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel6    ,bias_initializer=bias6    ) (layer21)
    #layer23    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel7    ,bias_initializer=bias7    ) (layer22)
    #layer24    = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel8    ,bias_initializer=bias8    ) (layer23)
    #layer31    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel9    ,bias_initializer=bias9    ) (inputs)
    #layer32    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel10   ,bias_initializer=bias10   ) (layer31)
    #layer33    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel11   ,bias_initializer=bias11   ) (layer32)
    #layer34    = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel12   ,bias_initializer=bias12   ) (layer33)
    #layer41    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel13   ,bias_initializer=bias13   ) (inputs)
    #layer42    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel14   ,bias_initializer=bias14   ) (layer41)
    #layer43    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel15   ,bias_initializer=bias15   ) (layer42)
    #layer44    = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel16   ,bias_initializer=bias16   ) (layer43)
    #layer51    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel17   ,bias_initializer=bias17   ) (inputs)
    #layer52    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel18   ,bias_initializer=bias18   ) (layer51)
    #layer53    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel19   ,bias_initializer=bias19   ) (layer52)
    #layer54    = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel20   ,bias_initializer=bias20   ) (layer53)
    #layer61    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel21   ,bias_initializer=bias21   ) (inputs)
    #layer62    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel22   ,bias_initializer=bias22   ) (layer61)
    #layer63    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel23   ,bias_initializer=bias23   ) (layer62)
    #layer64    = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel24   ,bias_initializer=bias24   ) (layer63)
    #layer71    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel25   ,bias_initializer=bias25   ) (inputs)
    #layer72    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel26   ,bias_initializer=bias26   ) (layer71)
    #layer73    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel27   ,bias_initializer=bias27   ) (layer72)
    #layer74    = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel28   ,bias_initializer=bias28   ) (layer73)
    #layer81    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel29   ,bias_initializer=bias29   ) (inputs)
    #layer82    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel30   ,bias_initializer=bias30   ) (layer81)
    #layer83    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel31   ,bias_initializer=bias31   ) (layer82)
    #layer84    = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel32   ,bias_initializer=bias32   ) (layer83)
    #layer91    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel33   ,bias_initializer=bias33   ) (inputs)
    #layer92    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel34   ,bias_initializer=bias34   ) (layer91)
    #layer93    = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel35   ,bias_initializer=bias35   ) (layer92)
    #layer94    = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel36   ,bias_initializer=bias36   ) (layer93)
    #layer101   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel37   ,bias_initializer=bias37   ) (inputs)
    #layer102   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel38   ,bias_initializer=bias38   ) (layer101)
    #layer103   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel39   ,bias_initializer=bias39   ) (layer102)
    #layer104   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel40   ,bias_initializer=bias40   ) (layer103)
    #layer111   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel41   ,bias_initializer=bias41   ) (inputs)
    #layer112   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel42   ,bias_initializer=bias42   ) (layer111)
    #layer113   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel43   ,bias_initializer=bias43   ) (layer112)
    #layer114   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel44   ,bias_initializer=bias44   ) (layer113)
    #layer121   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel45   ,bias_initializer=bias45   ) (inputs)
    #layer122   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel46   ,bias_initializer=bias46   ) (layer121)
    #layer123   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel47   ,bias_initializer=bias47   ) (layer122)
    #layer124   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel48   ,bias_initializer=bias48   ) (layer123)
    #layer131   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel49   ,bias_initializer=bias49   ) (inputs)
    #layer132   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel50   ,bias_initializer=bias50   ) (layer131)
    #layer133   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel51   ,bias_initializer=bias51   ) (layer132)
    #layer134   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel52   ,bias_initializer=bias52   ) (layer133)
    #layer141   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel53   ,bias_initializer=bias53   ) (inputs)
    #layer142   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel54   ,bias_initializer=bias54   ) (layer141)
    #layer143   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel55   ,bias_initializer=bias55   ) (layer142)
    #layer144   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel56   ,bias_initializer=bias56   ) (layer143)
    #layer151   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel57   ,bias_initializer=bias57   ) (inputs)
    #layer152   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel58   ,bias_initializer=bias58   ) (layer151)
    #layer153   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel59   ,bias_initializer=bias59   ) (layer152)
    #layer154   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel60   ,bias_initializer=bias60   ) (layer153)
    #layer161   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel61   ,bias_initializer=bias61   ) (inputs)
    #layer162   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel62   ,bias_initializer=bias62   ) (layer161)
    #layer163   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel63   ,bias_initializer=bias63   ) (layer162)
    #layer164   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel64   ,bias_initializer=bias64   ) (layer163)
    #layer171   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel65   ,bias_initializer=bias65   ) (inputs)
    #layer172   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel66   ,bias_initializer=bias66   ) (layer171)
    #layer173   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel67   ,bias_initializer=bias67   ) (layer172)
    #layer174   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel68   ,bias_initializer=bias68   ) (layer173)
    #layer181   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel69   ,bias_initializer=bias69   ) (inputs)
    #layer182   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel70   ,bias_initializer=bias70   ) (layer181)
    #layer183   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel71   ,bias_initializer=bias71   ) (layer182)
    #layer184   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel72   ,bias_initializer=bias72   ) (layer183)
    #layer191   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel73   ,bias_initializer=bias73   ) (inputs)
    #layer192   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel74   ,bias_initializer=bias74   ) (layer191)
    #layer193   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel75   ,bias_initializer=bias75   ) (layer192)
    #layer194   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel76   ,bias_initializer=bias76   ) (layer193)
    #layer201   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel77   ,bias_initializer=bias77   ) (inputs)
    #layer202   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel78   ,bias_initializer=bias78   ) (layer201)
    #layer203   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel79   ,bias_initializer=bias79   ) (layer202)
    #layer204   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel80   ,bias_initializer=bias80   ) (layer203)
    #layer211   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel81   ,bias_initializer=bias81   ) (inputs)
    #layer212   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel82   ,bias_initializer=bias82   ) (layer211)
    #layer213   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel83   ,bias_initializer=bias83   ) (layer212)
    #layer214   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel84   ,bias_initializer=bias84   ) (layer213)
    #layer221   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel85   ,bias_initializer=bias85   ) (inputs)
    #layer222   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel86   ,bias_initializer=bias86   ) (layer221)
    #layer223   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel87   ,bias_initializer=bias87   ) (layer222)
    #layer224   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel88   ,bias_initializer=bias88   ) (layer223)
    #layer231   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel89   ,bias_initializer=bias89   ) (inputs)
    #layer232   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel90   ,bias_initializer=bias90   ) (layer231)
    #layer233   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel91   ,bias_initializer=bias91   ) (layer232)
    #layer234   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel92   ,bias_initializer=bias92   ) (layer233)
    #layer241   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel93   ,bias_initializer=bias93   ) (inputs)
    #layer242   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel94   ,bias_initializer=bias94   ) (layer241)
    #layer243   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel95   ,bias_initializer=bias95   ) (layer242)
    #layer244   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel96   ,bias_initializer=bias96   ) (layer243)
    #layer251   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel97   ,bias_initializer=bias97   ) (inputs)
    #layer252   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel98   ,bias_initializer=bias98   ) (layer251)
    #layer253   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel99   ,bias_initializer=bias99   ) (layer252)
    #layer254   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel100  ,bias_initializer=bias100  ) (layer253)
    #layer261   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel101  ,bias_initializer=bias101  ) (inputs)
    #layer262   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel102  ,bias_initializer=bias102  ) (layer261)
    #layer263   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel103  ,bias_initializer=bias103  ) (layer262)
    #layer264   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel104  ,bias_initializer=bias104  ) (layer263)
    #layer271   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel105  ,bias_initializer=bias105  ) (inputs)
    #layer272   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel106  ,bias_initializer=bias106  ) (layer271)
    #layer273   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel107  ,bias_initializer=bias107  ) (layer272)
    #layer274   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel108  ,bias_initializer=bias108  ) (layer273)
    #layer281   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel109  ,bias_initializer=bias109  ) (inputs)
    #layer282   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel110  ,bias_initializer=bias110  ) (layer281)
    #layer283   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel111  ,bias_initializer=bias111  ) (layer282)
    #layer284   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel112  ,bias_initializer=bias112  ) (layer283)
    #layer291   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel113  ,bias_initializer=bias113  ) (inputs)
    #layer292   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel114  ,bias_initializer=bias114  ) (layer291)
    #layer293   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel115  ,bias_initializer=bias115  ) (layer292)
    #layer294   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel116  ,bias_initializer=bias116  ) (layer293)
    #layer301   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel117  ,bias_initializer=bias117  ) (inputs)
    #layer302   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel118  ,bias_initializer=bias118  ) (layer301)
    #layer303   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel119  ,bias_initializer=bias119  ) (layer302)
    #layer304   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel120  ,bias_initializer=bias120  ) (layer303)
    #layer311   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel121  ,bias_initializer=bias121  ) (inputs)
    #layer312   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel122  ,bias_initializer=bias122  ) (layer11)
    #layer313   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel123  ,bias_initializer=bias123  ) (layer312)
    #layer314   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel124  ,bias_initializer=bias124  ) (layer313)
    #layer321   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel125  ,bias_initializer=bias125  ) (inputs)
    #layer322   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel126  ,bias_initializer=bias126  ) (layer321)
    #layer323   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel127  ,bias_initializer=bias127  ) (layer322)
    #layer324   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel128  ,bias_initializer=bias128  ) (layer323)
    #layer331   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel129  ,bias_initializer=bias129  ) (inputs)
    #layer332   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel130  ,bias_initializer=bias130  ) (layer331)
    #layer333   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel131  ,bias_initializer=bias131  ) (layer332)
    #layer334   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel132  ,bias_initializer=bias132  ) (layer333)
    #layer341   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel133  ,bias_initializer=bias133  ) (inputs)
    #layer342   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel134  ,bias_initializer=bias134  ) (layer341)
    #layer343   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel135  ,bias_initializer=bias135  ) (layer342)
    #layer344   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel136  ,bias_initializer=bias136  ) (layer343)
    #layer351   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel137  ,bias_initializer=bias137  ) (inputs)
    #layer352   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel138  ,bias_initializer=bias138  ) (layer351)
    #layer353   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel139  ,bias_initializer=bias139  ) (layer352)
    #layer354   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel140  ,bias_initializer=bias140  ) (layer353)
    #layer361   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel141  ,bias_initializer=bias141  ) (inputs)
    #layer362   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel142  ,bias_initializer=bias142  ) (layer361)
    #layer363   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel143  ,bias_initializer=bias143  ) (layer362)
    #layer364   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel144  ,bias_initializer=bias144  ) (layer363)
    #layer371   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel145  ,bias_initializer=bias145  ) (inputs)
    #layer372   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel146  ,bias_initializer=bias146  ) (layer371)
    #layer373   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel147  ,bias_initializer=bias147  ) (layer372)
    #layer374   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel148  ,bias_initializer=bias148  ) (layer373)
    #layer381   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel149  ,bias_initializer=bias149  ) (inputs)
    #layer382   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel150  ,bias_initializer=bias150  ) (layer381)
    #layer383   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel151  ,bias_initializer=bias151  ) (layer382)
    #layer384   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel152  ,bias_initializer=bias152  ) (layer383)
    #layer391   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel153  ,bias_initializer=bias153  ) (inputs)
    #layer392   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel154  ,bias_initializer=bias154  ) (layer391)
    #layer393   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel155  ,bias_initializer=bias155  ) (layer392)
    #layer394   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel156  ,bias_initializer=bias156  ) (layer393)
    #layer401   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel157  ,bias_initializer=bias157  ) (inputs)
    #layer402   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel158  ,bias_initializer=bias158  ) (layer401)
    #layer403   = tf.keras.layers.Dense(240, activation='relu', kernel_initializer=kernel159  ,bias_initializer=bias159  ) (layer402)
    #layer404   = tf.keras.layers.Dense( 12,                    kernel_initializer=kernel160  ,bias_initializer=bias160  ) (layer403)
    
    
    #model = tf.keras.Model(
    #    inputs = inputs,
    #    outputs=[layer13,  layer23,  layer33,  layer43,  layer53,  layer63,  layer73,  layer83,  layer93,  layer103, 
    #             layer113, layer123, layer133, layer143, layer153, layer163, layer173, layer183, layer193, layer203, 
    #             layer213, layer223, layer233, layer243, layer253, layer263, layer273, layer283, layer293, layer303, 
    #             layer313, layer323, layer333, layer343, layer353, layer363, layer373, layer383, layer393, layer403],        
    #    name="DirectRankEstimator1"
    #    )

    opt = keras.optimizers.Adam(learning_rate=0.001)
    model.compile(optimizer=opt,
                  loss='mse',
                  metrics=['accuracy'])
                  
    model(np.zeros(shape=(1, 3312), dtype=np.float32))
    
    
    #path of the directory where you want to save your model
    frozen_out_path = ""
    # name of the .pb file
    frozen_graph_filename = "StateEvaluator"
    
    # Convert Keras model to ConcreteFunction
    full_model = tf.function(lambda x: model(x))
    full_model = full_model.get_concrete_function(
        tf.TensorSpec(model.inputs[0].shape, model.inputs[0].dtype))
    # Get frozen ConcreteFunction
    frozen_func = convert_variables_to_constants_v2(full_model)
    frozen_func.graph.as_graph_def()
    layers = [op.name for op in frozen_func.graph.get_operations()]
    print("-" * 60)
    print("Frozen model layers: ")
    for layer in layers:
        print(layer)
    print("-" * 60)
    print("Frozen model inputs: ")
    print(frozen_func.inputs)
    print("Frozen model outputs: ")
    print(frozen_func.outputs)
    # Save frozen graph to disk
    tf.io.write_graph(graph_or_graph_def=frozen_func.graph,
                    logdir=frozen_out_path,
                    name=f"{frozen_graph_filename}.pb",
                    as_text=False)
    # Save its text representation
    tf.io.write_graph(graph_or_graph_def=frozen_func.graph,
                    logdir=frozen_out_path,
                    name=f"{frozen_graph_filename}.pbtxt",
                    as_text=True)
    
    
    
    
    sess = tf.compat.v1.Session()
    tf.io.write_graph(sess.graph.as_graph_def(),".","tensorflowModel.pbtxt", as_text=True)
    converter = tf.lite.TFLiteConverter.from_keras_model(model)
    converter.target_spec.supported_ops = [
        tf.lite.OpsSet.TFLITE_BUILTINS#, 
        #tf.lite.OpsSet.SELECT_TF_OPS
    ]
    converter.allow_custom_ops = False
    converter.experimental_new_converter = True
    tflite_model = converter.convert()
    open("StateEvaluator.tflite", "wb").write(tflite_model)
    
    

    model.save('StateEvaluator.h5', save_format='h5')