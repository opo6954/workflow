using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMining.Core.Data
{
    /**
    * 순차 입력을 담당하는 최상위 인터페이스
    */
    public interface StreamReader<T> : DataReader
    {
        /**
         * 다음 데이터가 존재하는지 여부를 반환한다
         * 
         * @return  다음 데이터가 존재하는 여부
         */
        bool moveNext();

        /**
         * 다음 데이터를 반환한다.
         * 
         * @return  다음 데이터
         * @remark  이 메소드는 예외를 발생시킬 가능성이 있음
         */
        T next();

        /**
         * 현재 위치를 반환한다
         * 
         * @return  현재 위치 - 의미는 구체적인 구현에 따라 다름
         */
        int getIndex();

        /**
         * 데이터의 전체 개수를 반환한다. 알 수 없으면 -1을 반환한다.
         * 
         * @return  전체 데이터의 개수 - 의미는 구체적인 구현에 따라 다름
         */
        int getCount();
    }
}
