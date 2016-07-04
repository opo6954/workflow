using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMining.Core.Data
{
    /**
     * 데이터 입력을 담당하는 최상위 인터페이스
     */
    public interface DataReader
    {
        /**
         * 데이터 입력 준비를 시작한다.
         * 
         * @remark  이 메소드는 예외를 발생시킬 가능성이 있음
         */
        void open();

        /**
         * 데이터 입력을 종료하고 리소스를 정리한다
         * 
         * @remark  이 메소드는 예외를 발생시킬 가능성이 있음
         */
        void close();

        /**
         * 완전한 데이터 객체를 얻어낸다. 이 객체의 타입 및 처리 방법은
         * 구체적인 구현에 따른다.
         */
        Object getData();
    }
}
